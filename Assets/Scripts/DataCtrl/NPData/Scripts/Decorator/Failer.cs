//------------------------------------------------------------
// File: Failer.cs
// Created: 2025-12-05
// Purpose: Decorator that forces its child result to be failure.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public class Failer : Decorator
    {
        public Failer(Node decoratee) : base("Failer", decoratee)
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
            Stopped(false);
        }
    }

}