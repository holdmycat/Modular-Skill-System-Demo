//------------------------------------------------------------
// File: WaitUntilStopped.cs
// Created: 2025-12-05
// Purpose: Task that stays running until explicitly stopped.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
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