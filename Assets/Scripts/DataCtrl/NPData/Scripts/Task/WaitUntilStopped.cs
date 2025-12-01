using UnityEngine;
namespace Ebonor.DataCtrl
{
    public class WaitUntilStopped : Task
    {
        bool sucessWhenStopped;
        public WaitUntilStopped(bool sucessWhenStopped = false) : base("WaitUntilStopped")
        {
            this.sucessWhenStopped = sucessWhenStopped;
        }

        protected override void DoStart()
        {
            
// #if UNITY_EDITOR
//             Debug.Log("WaitUntilStopped.DoStart");
// #endif
        }
        
        protected override void DoStop()
        {
// #if UNITY_EDITOR
//             Debug.Log("WaitUntilStopped.DoStop");
// #endif
            Stopped(sucessWhenStopped);
        }
    }
}