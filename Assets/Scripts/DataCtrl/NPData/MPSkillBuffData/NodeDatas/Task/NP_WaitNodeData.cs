namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_WaitNodeData : NP_NodeDataBase
    {
        private Wait m_WaitNode;
        
        public NP_BlackBoardRelationData NPBalckBoardRelationData = new NP_BlackBoardRelationData();
        
        public override Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
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
    }
}
