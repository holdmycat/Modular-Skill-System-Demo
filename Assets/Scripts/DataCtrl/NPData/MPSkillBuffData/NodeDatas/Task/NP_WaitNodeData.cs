namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_WaitNodeData : NP_NodeDataBase, INPExecuteOnData
    {
        private Wait m_WaitNode;
        
        public NP_BlackBoardRelationData NPBalckBoardRelationData = new NP_BlackBoardRelationData();

        public eMPNetPosition ExecuteOn { get; set; } =
            eMPNetPosition.eServerOnly | eMPNetPosition.eLocalPlayer | eMPNetPosition.eHost;

        private static void NoopAction()
        {
        }
        
        public override Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            if (!ShouldExecute(runtimeTree))
                return new Action(NoopAction);
            this.m_WaitNode = new Wait(this.NPBalckBoardRelationData.BBKey);
            return this.m_WaitNode;
        }

        public override Task CreateNGTask<T>(string uId, T runtimeTree)
        {
            this.m_WaitNode = new Wait(this.NPBalckBoardRelationData.BBKey);
            return this.m_WaitNode;
        }
       
        
        public override Node NP_GetNode()
        {
            return this.m_WaitNode;
        }

        private bool ShouldExecute(NP_RuntimeTree runtimeTree)
        {
            if (ExecuteOn == eMPNetPosition.eNULL)
                return false;
            if (runtimeTree == null || runtimeTree.Context == null)
                return true;
            return (ExecuteOn & runtimeTree.Context.netPosition) != 0;
        }
    }
}
