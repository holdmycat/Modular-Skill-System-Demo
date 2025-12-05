//------------------------------------------------------------
// File: Service.cs
// Created: 2025-12-05
// Purpose: Decorator that runs a service callback alongside a child.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using UnityEngine;
namespace Ebonor.DataCtrl
{
    public class Service : Decorator
    {
        private System.Action serviceMethod;

        private float interval = -1.0f;
        private float randomVariation;
        private NP_ClassForStoreAction mStoreAction;
        public Service(float interval, float randomVariation, System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = randomVariation;

            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }
        
        public Service(float interval, System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = interval * 0.05f;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(NP_ClassForStoreAction storeAction, Node decoratee) : base("Service", decoratee)
        {
            mStoreAction = storeAction;
            this.serviceMethod = storeAction.GetActionToBeDone();
            this.interval = storeAction.GetInternal();
            this.randomVariation = interval * 0.05f;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }
        
        public Service(System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.Label = "every tick";
        }

        protected override void DoStart()
        {
            if (null != mStoreAction)
            {
                this.interval = mStoreAction.GetInternal();
            }
           
            if (this.interval <= 0f)
            {
                this.Clock.AddUpdateObserver(serviceMethod);
                serviceMethod();
            }
            else if (randomVariation <= 0f)
            {
                this.Clock.AddTimer(this.interval, -1, serviceMethod);
                serviceMethod();
            }
            else
            {
                InvokeServiceMethodWithRandomVariation();
            }
            Decoratee.Start();
        }

        protected override void DoStop()
        {
            //Debug.LogFormat("Server, DoStop");
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            //Debug.LogFormat("Server, DoChildStopped, result:{0}", result);
            if (this.interval <= 0f)
            {
                this.Clock.RemoveUpdateObserver(serviceMethod);
            }
            else if (randomVariation <= 0f)
            {
                this.Clock.RemoveTimer(serviceMethod);
            }
            else
            {
                this.Clock.RemoveTimer(InvokeServiceMethodWithRandomVariation);
            }
            Stopped(result);
        }
        
        private void InvokeServiceMethodWithRandomVariation()
        {
            //Debug.LogFormat("InvokeServiceMethodWithRandomVariation, interval:{0}", interval);
            serviceMethod();
            this.Clock.AddTimer(interval, randomVariation, 0, InvokeServiceMethodWithRandomVariation);
        }
    }
}