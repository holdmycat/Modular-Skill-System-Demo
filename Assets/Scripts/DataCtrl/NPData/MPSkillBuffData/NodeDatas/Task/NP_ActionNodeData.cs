namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_ActionNodeData : NP_NodeDataBase
    {
        private Action m_ActionNode;

        [UnityEngine.SerializeReference]
        public NP_ClassForStoreAction NpClassForStoreAction;

        public override NP_NodeDataBase Clone()
        {
            var copy = (NP_ActionNodeData)base.Clone();
            copy.NpClassForStoreAction = NpClassForStoreAction?.Clone();
            copy.m_ActionNode = null;
            return copy;
        }

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
