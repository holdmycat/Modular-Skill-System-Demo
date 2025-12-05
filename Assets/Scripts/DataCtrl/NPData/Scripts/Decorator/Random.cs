//------------------------------------------------------------
// File: Random.cs
// Created: 2025-12-05
// Purpose: Decorator that randomly selects whether to run its child.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public class Random : Decorator
    {
        private float probability;

        public Random(float probability, Node decoratee) : base("Random", decoratee)
        {
            this.probability = probability;
        }

        protected override void DoStart()
        {
            if (UnityEngine.Random.value <= this.probability)
            {
                Decoratee.Start();
            }
            else
            {
                Stopped(false);
            }
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}