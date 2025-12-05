//------------------------------------------------------------
// File: Action.cs
// Created: 2025-12-05
// Purpose: Task wrapper for synchronous and multi-frame delegate actions.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using UnityEngine.Assertions;

namespace Ebonor.DataCtrl
{
    public class Action : Task
    {
        public enum Result
        {
            SUCCESS,
            FAILED,
            BLOCKED,
            PROGRESS
        }

        public enum Request
        {
            START,
            UPDATE,
            CANCEL,
        }

        private System.Func<bool> singleFrameFunc = null;
        private System.Func<bool, Result> multiFrameFunc1 = null;
        private System.Func<Request, Result> multiFrameFunc2 = null;
        private System.Action action = null;
#if UNITY_EDITOR
        
       
       
#endif
        public System.Action actionInst => action;
        public System.Func<bool> singleFrameFuncInst => singleFrameFunc;
        
        public System.Func<bool, Result> multiFrameFunc1Inst => multiFrameFunc1;
        public System.Func<Request, Result> multiFrameFunc2Inst => multiFrameFunc2;
        
        private bool bWasBlocked = false;

        public Action(System.Action action) : base("Action")
        {
            this.action = action;
        }

        public Action(System.Func<bool, Result> multiframeFunc1) : base("Action")
        {
            this.multiFrameFunc1 = multiframeFunc1;
        }

        public Action(System.Func<Request, Result> multiframeFunc2) : base("Action")
        {
            this.multiFrameFunc2 = multiframeFunc2;
        }


        public Action(System.Func<bool> singleFrameFunc) : base("Action")
        {
            this.singleFrameFunc = singleFrameFunc;
        }

        protected override void DoStart()
        {
            if (this.action != null)
            {
                this.action.Invoke();
                this.Stopped(true);
            }
            else if (this.multiFrameFunc1 != null)
            {
                Result result = this.multiFrameFunc1.Invoke(false);
                if ( result == Result.PROGRESS )
                {
                    this.RootNode.Clock.AddUpdateObserver( OnUpdateFunc );
                }
                else if ( result == Result.BLOCKED )
                {
                    this.bWasBlocked = true;
                    this.RootNode.Clock.AddUpdateObserver( OnUpdateFunc );
                }
                else
                {
                    this.Stopped(result == Result.SUCCESS);
                }
            }
            else if (this.multiFrameFunc2 != null)
            {
                Result result = this.multiFrameFunc2.Invoke(Request.START);
                if (result == Result.PROGRESS)
                {
                    this.RootNode.Clock.AddUpdateObserver(OnUpdateFunc2);
                }
                else if ( result == Result.BLOCKED )
                {
                    this.bWasBlocked = true;
                    this.RootNode.Clock.AddUpdateObserver( OnUpdateFunc2 );
                }
                else
                {
                    this.Stopped(result == Result.SUCCESS);
                }
            }
            else if (this.singleFrameFunc != null)
            {
                this.Stopped(this.singleFrameFunc.Invoke());
            }
        }

        private void OnUpdateFunc()
        {
            Result result = this.multiFrameFunc1.Invoke(false);
            if (result != Result.PROGRESS && result != Result.BLOCKED)
            {
                this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
                this.Stopped(result == Result.SUCCESS);
            }
        }

        private void OnUpdateFunc2()
        {
            Result result = this.multiFrameFunc2.Invoke( bWasBlocked ? Request.START : Request.UPDATE);

            if ( result == Result.BLOCKED )
            {
                bWasBlocked = true;
            }
            else if ( result == Result.PROGRESS )
            {
                bWasBlocked = false;
            }
            else
            {
                this.RootNode.Clock.RemoveUpdateObserver( OnUpdateFunc2 );
                this.Stopped( result == Result.SUCCESS );
            }
        }

        protected override void DoStop()
        {
            if (this.multiFrameFunc1 != null)
            {
                Result result = this.multiFrameFunc1.Invoke(true);
                //Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS, Result.FAILED/BLOCKED after beeing cancelled!");
                this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc);
                this.Stopped(result == Result.SUCCESS);
            }
            else if (this.multiFrameFunc2 != null)
            {
                Result result = this.multiFrameFunc2.Invoke(Request.CANCEL);
                //Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS or Result.FAILED/BLOCKED after beeing cancelled!");
                this.RootNode.Clock.RemoveUpdateObserver(OnUpdateFunc2);
                this.Stopped(result == Result.SUCCESS);
            }
            else
            {
                Assert.IsTrue(false, "DoStop called for a single frame action on " + this);
            }
        }
    }
}