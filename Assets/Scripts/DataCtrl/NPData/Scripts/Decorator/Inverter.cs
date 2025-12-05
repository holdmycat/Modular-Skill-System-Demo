//------------------------------------------------------------
// File: Inverter.cs
// Created: 2025-12-05
// Purpose: Decorator that inverts the success state of its child.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
namespace Ebonor.DataCtrl
{
    public class Inverter : Decorator
    {
        public Inverter(Node decoratee) : base("Inverter", decoratee)
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
            Stopped(!result);
        }
    }
}