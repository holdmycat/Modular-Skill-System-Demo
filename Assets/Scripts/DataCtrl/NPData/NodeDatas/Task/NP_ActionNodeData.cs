//------------------------------------------------------------
// File: NP_ActionNodeData.cs
// Created: 2025-12-01
// Purpose: Action node data configuration.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
    public class NP_ActionNodeData : NP_NodeDataBase
    {
        private Action m_ActionNode;

        public NP_ClassForStoreAction NpClassForStoreAction;

        public override Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            this.NpClassForStoreAction.BelongToUnit = unit;
            this.NpClassForStoreAction.BelongtoRuntimeTree = runtimeTree;
            
            this.m_ActionNode = this.NpClassForStoreAction._CreateNPBehaveAction();
            
            return this.m_ActionNode;
        }

        
        public override Node NP_GetNode()
        {
            return this.m_ActionNode;
        }
    }
}
