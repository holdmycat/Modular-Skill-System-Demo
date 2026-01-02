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

        public override Task CreateNGTask<T>(string uId, T runtimeTree)
        {
            this.NpClassForStoreAction.SetNGRuntimeTree(uId, runtimeTree);
            this.m_ActionNode = this.NpClassForStoreAction._CreateNPBehaveAction();
            return this.m_ActionNode;
        }
        
        
        
        public override Node NP_GetNode()
        {
            return this.m_ActionNode;
        }
    }
}
