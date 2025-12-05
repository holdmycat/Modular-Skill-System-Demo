//------------------------------------------------------------
// File: Clock.cs
// Created: 2025-12-05
// Purpose: Lightweight scheduler handling timed callbacks for behavior tree nodes.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using Ebonor.Framework;
using Unity.Profiling;
using UnityEngine.Assertions;

namespace Ebonor.DataCtrl
{

    public class Clock
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Clock));
        private List<System.Action> updateObservers = new List<System.Action>();
        private Dictionary<System.Action, Timer> timers = new Dictionary<System.Action, Timer>();
        private HashSet<System.Action> removeObservers = new HashSet<System.Action>();
        private HashSet<System.Action> addObservers = new HashSet<System.Action>();
        private HashSet<System.Action> removeTimers = new HashSet<System.Action>();
        private Dictionary<System.Action, Timer> addTimers = new Dictionary<System.Action, Timer>();
        private List<System.Action> listActions = new List<System.Action>();
        private bool isInUpdate = false;

        class Timer
        {
            public double absoluteTime = 0f;
            public double time = 0f;
            public int repeat = 0;
            public bool used = false;
        }

        private double elapsedTime = 0f;

        private List<Timer> timerPool = new List<Timer>();
        private int currentTimerPoolIndex = 0;

        /// <summary>Register a timer function</summary>
        /// <param name="time">time in milliseconds</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float time, int repeat, System.Action action)
        {
            AddTimer(time, 0f, repeat, action);
        }

        /// <summary>Register a timer function with random variance</summary>
        /// <param name="time">time in milliseconds</param>
        /// <param name="randomVariance">deviate from time on a random basis</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float time, float randomVariance, int repeat, System.Action action)
        {
            var duration = time;
            time = time - randomVariance * 0.5f + randomVariance * UnityEngine.Random.value;
            if (!isInUpdate)
            {
                if (this.timers.ContainsKey(action))
                {
                    Assert.IsTrue(this.timers[action].used);
                    this.timers[action].absoluteTime = elapsedTime + time;
                    this.timers[action].repeat = repeat;
                    this.timers[action].time = duration;
                }
                else
                {
                    this.timers[action] = getTimerFromPool(elapsedTime + time, repeat, duration);
                }
            }
            else
            {
                if (!this.addTimers.ContainsKey(action))
                {
                    this.addTimers[action] = getTimerFromPool(elapsedTime + time, repeat, duration);
                }
                else
                {
                    Assert.IsTrue(this.addTimers[action].used);
                    this.addTimers[action].repeat = repeat;
                    this.addTimers[action].absoluteTime = elapsedTime + time;
                    this.addTimers[action].time = duration;
                }

                if (this.removeTimers.Contains(action))
                {
                    this.removeTimers.Remove(action);
                }
            }
        }

        public void RemoveTimer(System.Action action)
        {
            if (!isInUpdate)
            {
                if (this.timers.ContainsKey(action))
                {
                    timers[action].used = false;
                    this.timers.Remove(action);
                }
            }
            else
            {
                if (this.timers.ContainsKey(action))
                {
                    this.removeTimers.Add(action);
                }
                if (this.addTimers.ContainsKey(action))
                {
                    Assert.IsTrue(this.addTimers[action].used);
                    this.addTimers[action].used = false;
                    this.addTimers.Remove(action);
                }
            }
        }

        public bool HasTimer(System.Action action)
        {
            if (!isInUpdate)
            {
                return this.timers.ContainsKey(action);
            }
            else
            {
                if (this.removeTimers.Contains(action))
                {
                    return false;
                }
                else if (this.addTimers.ContainsKey(action))
                {
                    return true;
                }
                else
                {
                    return this.timers.ContainsKey(action);
                }
            }
        }

        /// <summary>Register a function that is called every frame</summary>
        /// <param name="action">function to invoke</param>
        public void AddUpdateObserver(System.Action action)
        {
            if (!isInUpdate)
            {
                this.updateObservers.Add(action);
            }
            else
            {
                if (!this.updateObservers.Contains(action))
                {
                    this.addObservers.Add(action);
                }
                if (this.removeObservers.Contains(action))
                {
                    this.removeObservers.Remove(action);
                }
            }
        }

        public void RemoveUpdateObserver(System.Action action)
        {
            if (!isInUpdate)
            {
                this.updateObservers.Remove(action);
            }
            else
            {
                if (this.updateObservers.Contains(action))
                {
                    this.removeObservers.Add(action);
                }
                if (this.addObservers.Contains(action))
                {
                    this.addObservers.Remove(action);
                }
            }
        }
        
        private static readonly ProfilerMarker k_updateObserversMarker               = new ProfilerMarker("Clock/OnFixedUpdate/updateObserversUpdate");
        private static readonly ProfilerMarker k_updatetimersMarker               = new ProfilerMarker("Clock/OnFixedUpdate/updateTimersUpdate");
        private static readonly ProfilerMarker k_addObserversMarker               = new ProfilerMarker("Clock/OnFixedUpdate/addObserversUpdate");
        private static readonly ProfilerMarker k_removeObserversMarker               = new ProfilerMarker("Clock/OnFixedUpdate/removeObserversUpdate");
        private static readonly ProfilerMarker k_addTimersMarker               = new ProfilerMarker("Clock/OnFixedUpdate/addTimersUpdate");
        private static readonly ProfilerMarker k_removeTimersMarker               = new ProfilerMarker("Clock/OnFixedUpdate/removeTimersUpdate");
        private static readonly ProfilerMarker k_endClearMarker               = new ProfilerMarker("Clock/OnFixedUpdate/endClearUpdate");

        public void OnFixedUpdate(float deltaTime)
        {
            this.elapsedTime += deltaTime;

            this.isInUpdate = true;

            k_updateObserversMarker.Begin();
            foreach (System.Action action in updateObservers)
            {
                if (!removeObservers.Contains(action))
                {
                    action.Invoke();
                }
            }
            k_updateObserversMarker.End();

            k_updatetimersMarker.Begin();
            listActions.Clear();
            foreach (var variable in timers)
            {
                listActions.Add(variable.Key);
            }
            
            foreach (System.Action timer in listActions)
            {
                if (this.removeTimers.Contains(timer))
                {
                    continue;
                }

                Timer time = timers[timer];
                if (time.absoluteTime <= this.elapsedTime)
                {
                    if (time.repeat == 0)
                    {
                        RemoveTimer(timer);
                    }
                    else if (time.repeat > 0)
                    {
                        time.repeat--;
                        time.absoluteTime += time.time;
                    }
                    timer.Invoke();
                }
            }

            k_updatetimersMarker.End();
            
            k_addObserversMarker.Begin();
            foreach (System.Action action in this.addObservers)
            {
                this.updateObservers.Add(action);
            }
            k_addObserversMarker.End();
            
            k_removeObserversMarker.Begin();
            foreach (System.Action action in this.removeObservers)
            {
                this.updateObservers.Remove(action);
            }
            k_removeObserversMarker.End();
            
            k_addTimersMarker.Begin();
            foreach (System.Action action in this.addTimers.Keys)
            {
                if (this.timers.ContainsKey(action))
                {
                    //Assert.AreNotEqual(this.timers[action], this.addTimers[action]);
                    this.timers[action].used = false;
                }
                Assert.IsTrue(this.addTimers[action].used);
                this.timers[action] = this.addTimers[action];
            }
            k_addTimersMarker.End();
            
            k_removeTimersMarker.Begin();
            foreach (System.Action action in this.removeTimers)
            {
                Assert.IsTrue(this.timers[action].used);
                timers[action].used = false;
                this.timers.Remove(action);
            }
            k_removeTimersMarker.End();
            
            k_endClearMarker.Begin();
            this.addObservers.Clear();
            this.removeObservers.Clear();
            this.addTimers.Clear();
            this.removeTimers.Clear();
            k_endClearMarker.End();
            
            this.isInUpdate = false;
        }
        private Timer getTimerFromPool(double absoluteTime, int repeat, float time)
        {
            int i = 0;
            int l = timerPool.Count;
            Timer timer = null;
            while (i < l)
            {
                int timerIndex = (i + currentTimerPoolIndex) % l;
                if (!timerPool[timerIndex].used)
                {
                    currentTimerPoolIndex = timerIndex;
                    timer = timerPool[timerIndex];
                    break;
                }
                i++;
            }

            if (timer == null)
            {
                timer = new Timer();
                currentTimerPoolIndex = 0;
                timerPool.Add(timer);
            }

            timer.used = true;
            timer.absoluteTime = absoluteTime;
            timer.repeat = repeat;
            timer.time = time;
            return timer;
        }
        
    }
}