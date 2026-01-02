using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Action = Ebonor.DataCtrl.Action;

namespace Ebonor.GamePlay
{
    
    
    public class NP_WaitAction : NP_ClassForStoreAction
    {
        static readonly ILog log = LogManager.GetLogger(typeof(NP_WaitAction));

        [Header("等待时间(s)")]
        public float WaitDuration;
        
        private float mStartTime = 0f;
        
        public override Func<Action.Request, Action.Result> GetFunc2ToBeDone()
        {
            this.Func2 = this.DelayFrameFreeze;
            return this.Func2;
        }
        
        Action.Result DelayFrameFreeze(Action.Request _request)
        {
            switch (_request)
            {
                case DataCtrl.Action.Request.START:
                {
                  
                    mStartTime = 0f;
                    return Ebonor.DataCtrl.Action.Result.PROGRESS;
                }
                case DataCtrl.Action.Request.UPDATE:
                {
                    mStartTime += Time.fixedDeltaTime;
                    return mStartTime >= WaitDuration ? Ebonor.DataCtrl.Action.Result.SUCCESS : Ebonor.DataCtrl.Action.Result.PROGRESS;
                }
                case DataCtrl.Action.Request.CANCEL:
                {
                    return Ebonor.DataCtrl.Action.Result.SUCCESS;
                }
            }
            
            return Ebonor.DataCtrl.Action.Result.FAILED;
        }
    }
}
