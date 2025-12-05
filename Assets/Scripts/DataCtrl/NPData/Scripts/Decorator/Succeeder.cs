//------------------------------------------------------------
// File: Succeeder.cs
// Created: 2025-12-05
// Purpose: Decorator that forces its child result to succeed.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public class Succeeder : Decorator
    {
        public Succeeder(Node decoratee) : base("Succeeder", decoratee)
        {
        }

        protected override void DoStart()
        {
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(true);
        }
    }
}