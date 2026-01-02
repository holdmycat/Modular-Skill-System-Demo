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

        public override Task CreateNGTask<T>(string uId, T runtimeTree)
        {
            this.m_WaitUntilStopped = new WaitUntilStopped();
            return this.m_WaitUntilStopped;
        }
      
    }
}
