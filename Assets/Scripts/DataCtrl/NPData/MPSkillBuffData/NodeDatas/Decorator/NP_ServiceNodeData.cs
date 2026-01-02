using UnityEngine;
using Action = System.Action;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_ServiceNodeData: NP_NodeDataBase
    {
        public Service m_Service;
        
        public bool IsSelfSetInterval;
        
        public float interval;

        [SerializeReference]
        public NP_ClassForStoreAction NpClassForStoreAction = new NP_ClassForStoreAction();

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
        
        public override Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node)
        {
            this.NpClassForStoreAction.SetNGRuntimeTree<T>(unit, runtimeTree);
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
