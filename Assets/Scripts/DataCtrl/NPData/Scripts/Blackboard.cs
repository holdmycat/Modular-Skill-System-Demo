//------------------------------------------------------------
// File: Blackboard.cs
// Created: 2025-12-05
// Purpose: Shared blackboard storage with observer dispatching for behavior tree nodes.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class Blackboard
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Blackboard));
        
        public enum Type
        {
            ADD,
            REMOVE,
            CHANGE
        }
        private struct Notification
        {
            public string key;
            public Type type;
            public ANP_BBValue value;
            public Notification(string key, Type type, ANP_BBValue value)
            {
                this.key = key;
                this.type = type;
                this.value = value;
            }
        }

        private Clock clock;
        private Dictionary<string, ANP_BBValue> data = new Dictionary<string, ANP_BBValue>();
        private Dictionary<string, List<System.Action<Type, ANP_BBValue>>> observers = new Dictionary<string, List<System.Action<Type, ANP_BBValue>>>();
        private bool isNotifiyng = false;
        private Dictionary<string, List<System.Action<Type, ANP_BBValue>>> addObservers = new Dictionary<string, List<System.Action<Type, ANP_BBValue>>>();
        private Dictionary<string, List<System.Action<Type, ANP_BBValue>>> removeObservers = new Dictionary<string, List<System.Action<Type, ANP_BBValue>>>();
        private List<Notification> notifications = new List<Notification>();
        private List<Notification> notificationsDispatch = new List<Notification>();
        private Blackboard parentBlackboard;
        private HashSet<Blackboard> children = new HashSet<Blackboard>();

        public Blackboard(Blackboard parent, Clock clock)
        {
            this.clock = clock;
            this.parentBlackboard = parent;
        }
        public Blackboard(Clock clock)
        {
            this.parentBlackboard = null;
            this.clock = clock;
        }

        public void Enable()
        {
            if (this.parentBlackboard != null)
            {
                this.parentBlackboard.children.Add(this);
            }
        }

        public void Disable()
        {
            if (this.parentBlackboard != null)
            {
                this.parentBlackboard.children.Remove(this);
            }
            if (this.clock != null)
            {
                this.clock.RemoveTimer(this.NotifiyObservers);
            }
        }

        public ANP_BBValue this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }



        public void Set<T>(string key, T value)
        {
            
#if UNITY_EDITOR
            if (key.Equals(ConstData.BB_ISIDLE))
            {
                if (value is bool valueBool && valueBool.Equals(false))
                {
                    log.DebugFormat("Script, SetBlackBoardValue, BBKey:{0}, BBValue:{1}", key, valueBool);
                }
               
            }
#endif
            
            if (this.parentBlackboard != null && this.parentBlackboard.Isset(key))
            {
                this.parentBlackboard.Set(key, value);
            }
            else
            {
                if (!this.data.ContainsKey(key))
                {
                    ANP_BBValue newBBValue = NP_BBValueHelper.AutoCreateNPBBValueFromTValue(value);
                    this.data[key] = newBBValue;
                    this.notifications.Add(new Notification(key, Type.ADD, newBBValue));
                    this.clock.AddTimer(0f, 0, NotifiyObservers);
                }
                else
                {
                    NP_BBValueBase<T> targetBBValue = this.data[key] as NP_BBValueBase<T>;
                    if ((targetBBValue == null && value != null) ||
                        (targetBBValue != null &&
                         (targetBBValue.GetValue() == null || !targetBBValue.GetValue().Equals(value))))
                    {
                        targetBBValue.SetValueFrom(value);
                        this.notifications.Add(new Notification(key, Type.CHANGE, targetBBValue));
                        this.clock.AddTimer(0f, 0, NotifiyObservers);
                    }
                    // if ((this.data[key] == null && value != null) || (this.data[key] != null && !this.data[key].Equals(value)))
                    // {
                    //     this.data[key] = targetBBValue;
                    //     this.notifications.Add(new Notification(key, Type.CHANGE, targetBBValue));
                    //     this.clock.AddTimer(0f, 0, NotifiyObservers);
                    // }
                }
            }
        }

        public void Unset(string key)
        {
            if (this.data.ContainsKey(key))
            {
                this.data.Remove(key);
                this.notifications.Add(new Notification(key, Type.REMOVE, null));
                this.clock.AddTimer(0f, 0, NotifiyObservers);
            }
        }

        public Dictionary<string, ANP_BBValue> GetDatas()
        {
            return this.data;
        }

        public T Get<T>(string key)
        {
            ANP_BBValue result = Get(key);
            if (result == null)
            {
                return default(T);
            }
            NP_BBValueBase<T> finalResult = result as NP_BBValueBase<T>;
            if (finalResult == null)
            {
                log.Error($"Blackboard value cast failed. Key: {key}, Type: {typeof(NP_BBValueBase<T>)}");
                return default;
            }
            
            return finalResult.GetValue();
            
        }

        public ANP_BBValue Get(string key)
        {
            if (this.data.ContainsKey(key))
            {
                return data[key];
            }
            else if (this.parentBlackboard != null)
            {
                return this.parentBlackboard.Get(key);
            }
            else
            {
                return null;
            }
        }

        public bool Isset(string key)
        {
            return this.data.ContainsKey(key) || (this.parentBlackboard != null && this.parentBlackboard.Isset(key));
        }

        public void AddObserver(string key, System.Action<Type, ANP_BBValue> observer)
        {
            List<System.Action<Type, ANP_BBValue>> observers = GetObserverList(this.observers, key);
            if (!isNotifiyng)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
            }
            else
            {
                if (!observers.Contains(observer))
                {
                    List<System.Action<Type, ANP_BBValue>> addObservers = GetObserverList(this.addObservers, key);
                    if (!addObservers.Contains(observer))
                    {
                        addObservers.Add(observer);
                    }
                }

                List<System.Action<Type, ANP_BBValue>> removeObservers = GetObserverList(this.removeObservers, key);
                if (removeObservers.Contains(observer))
                {
                    removeObservers.Remove(observer);
                }
            }
        }

        public void RemoveObserver(string key, System.Action<Type, ANP_BBValue> observer)
        {
            List<System.Action<Type, ANP_BBValue>> observers = GetObserverList(this.observers, key);
            if (!isNotifiyng)
            {
                if (observers.Contains(observer))
                {
                    observers.Remove(observer);
                }
            }
            else
            {
                List<System.Action<Type, ANP_BBValue>> removeObservers = GetObserverList(this.removeObservers, key);
                if (!removeObservers.Contains(observer))
                {
                    if (observers.Contains(observer))
                    {
                        removeObservers.Add(observer);
                    }
                }

                List<System.Action<Type, ANP_BBValue>> addObservers = GetObserverList(this.addObservers, key);
                if (addObservers.Contains(observer))
                {
                    addObservers.Remove(observer);
                }
            }
        }


#if UNITY_EDITOR
        public List<string> Keys
        {
            get
            {
                if (this.parentBlackboard != null)
                {
                    List<string> keys = this.parentBlackboard.Keys;
                    keys.AddRange(data.Keys);
                    return keys;
                }
                else
                {
                    return new List<string>(data.Keys);
                }
            }
        }

        public int NumObservers
        {
            get
            {
                int count = 0;
                foreach (string key in observers.Keys)
                {
                    count += observers[key].Count;
                }
                return count;
            }
        }
#endif


        private void NotifiyObservers()
        {
            if (notifications.Count == 0)
            {
                return;
            }

            notificationsDispatch.Clear();
            notificationsDispatch.AddRange(notifications);
            foreach (Blackboard child in children)
            {
                child.notifications.AddRange(notifications);
                child.clock.AddTimer(0f, 0, child.NotifiyObservers);
            }
            notifications.Clear();

            isNotifiyng = true;
            foreach (Notification notification in notificationsDispatch)
            {
                if (!this.observers.ContainsKey(notification.key))
                {
                    //                Debug.Log("1 do not notify for key:" + notification.key + " value: " + notification.value);
                    continue;
                }

                List<System.Action<Type, ANP_BBValue>> observers = GetObserverList(this.observers, notification.key);
                foreach (System.Action<Type, ANP_BBValue> observer in observers)
                {
                    if (this.removeObservers.ContainsKey(notification.key) && this.removeObservers[notification.key].Contains(observer))
                    {
                        continue;
                    }
                    observer(notification.type, notification.value);
                }
            }

            foreach (string key in this.addObservers.Keys)
            {
                GetObserverList(this.observers, key).AddRange(this.addObservers[key]);
            }
            foreach (string key in this.removeObservers.Keys)
            {
                foreach (System.Action<Type, ANP_BBValue> action in removeObservers[key])
                {
                    GetObserverList(this.observers, key).Remove(action);
                }
            }
            this.addObservers.Clear();
            this.removeObservers.Clear();

            isNotifiyng = false;
        }

        private List<System.Action<Type, ANP_BBValue>> GetObserverList(Dictionary<string, List<System.Action<Type, ANP_BBValue>>> target, string key)
        {
            List<System.Action<Type, ANP_BBValue>> observers;
            if (target.ContainsKey(key))
            {
                observers = target[key];
            }
            else
            {
                observers = new List<System.Action<Type, ANP_BBValue>>();
                target[key] = observers;
            }
            return observers;
        }
    }
}
