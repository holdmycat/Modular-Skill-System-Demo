using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class TimeRepeater : Decorator
    {
        private float Duration = -1;

        /// <param name="loopCount">number of times to execute the decoratee. Set to -1 to repeat forever, be careful with endless loops!</param>
        /// <param name="decoratee">Decorated Node</param>
        public TimeRepeater(float interval, Node decoratee) : base("TimeRepeater", decoratee)
        {
            this.Duration = interval;
          
        }

        /// <param name="decoratee">Decorated Node, repeated forever</param>
        public TimeRepeater(Node decoratee) : base("TimeRepeater", decoratee)
        {
        }

        protected override void DoStart()
        {
            Decoratee.Start();
            // if (Time.time - currentTime >= Duration)
            // {
            //     currentTime = Time.time;
            //     Decoratee.Start();
            // }
            // else
            // {
            //     this.Stopped(true);
            // }
        }

        protected override void DoStop()
        {
            this.Clock.RemoveTimer(RestartDecoratee);

            //Debug.Log("Repeater, Decoratee.IsAlive: " + Decoratee.IsActive);
            
            if (Decoratee.IsActive)
            {
                Decoratee.Stop();
            }
            else
            {
                Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (result)
            {
                if (IsStopRequested)
                {
                    Stopped(true);
                }
                else
                {
                    this.Clock.AddTimer(Duration, 0, RestartDecoratee);
                }
            }
            else
            {
                Stopped(false);
            }
        }

        private void RestartDecoratee()
        {
            Decoratee.Start();
        }
    }
}