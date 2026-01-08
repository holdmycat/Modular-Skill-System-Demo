namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_WaitUntilStoppedData: NP_NodeDataBase, INPExecuteOnData
    {
        private WaitUntilStopped m_WaitUntilStopped;

        public eMPNetPosition ExecuteOn { get; set; } =
            eMPNetPosition.eServerOnly | eMPNetPosition.eLocalPlayer | eMPNetPosition.eHost;

        private static void NoopAction()
        {
        }

        public override Node NP_GetNode()
        {
            return this.m_WaitUntilStopped;
        }

        public override Task CreateTask(uint unit, NP_RuntimeTree runtimeTree)
        {
            if (!ShouldExecute(runtimeTree))
                return new Action(NoopAction);
            this.m_WaitUntilStopped = new WaitUntilStopped();
            return this.m_WaitUntilStopped;
        }

        public override Task CreateNGTask<T>(string uId, T runtimeTree)
        {
            this.m_WaitUntilStopped = new WaitUntilStopped();
            return this.m_WaitUntilStopped;
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
