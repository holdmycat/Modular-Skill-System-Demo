//------------------------------------------------------------
// File: NP_WaitUntilStoppedData.cs
// Created: 2025-12-01
// Purpose: Wait-until-stopped node data configuration.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
    public class NP_WaitUntilStoppedData: NP_NodeDataBase
    {
        private WaitUntilStopped m_WaitUntilStopped;

        public override Node NP_GetNode()
        {
            return this.m_WaitUntilStopped;
        }

        public override Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            this.m_WaitUntilStopped = new WaitUntilStopped();
            return this.m_WaitUntilStopped;
        }


      
    }
}
