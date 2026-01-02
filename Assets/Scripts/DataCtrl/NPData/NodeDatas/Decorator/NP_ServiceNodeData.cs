//------------------------------------------------------------
// File: NP_ServiceNodeData.cs
// Created: 2025-12-01
// Purpose: Service decorator node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using Action = System.Action;

namespace Ebonor.DataCtrl
{
    public class NP_ServiceNodeData: NP_NodeDataBase
    {
        public Service m_Service;
        
        public bool IsSelfSetInterval;
        
        public float interval;

        public NP_ClassForStoreAction NpClassForStoreAction;

        public override Node NP_GetNode()
        {
            return this.m_Service;
        }

        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            this.NpClassForStoreAction.BelongToUnit = unit;
            this.NpClassForStoreAction.BelongtoRuntimeTree = runtimeTree;
            
            if(IsSelfSetInterval){
                this.m_Service = new Service(interval, this.NpClassForStoreAction.GetActionToBeDone(), node);
            }
            else {
                this.m_Service = new Service(NpClassForStoreAction, node);
            }
            
            return this.m_Service;
        }
        
    }
}
