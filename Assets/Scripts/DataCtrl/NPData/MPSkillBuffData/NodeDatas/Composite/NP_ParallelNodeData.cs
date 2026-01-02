
namespace Ebonor.DataCtrl
{
    public class NP_ParallelNodeData: NP_NodeDataBase
    {
        private Parallel m_ParallelNode;

        public Parallel.Policy SuccessPolicy = Parallel.Policy.ALL;

        public Parallel.Policy FailurePolicy = Parallel.Policy.ALL;

        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_ParallelNode = new Parallel(SuccessPolicy, FailurePolicy, nodes);
            return this.m_ParallelNode;
        }

        public override Node NP_GetNode()
        {
            return this.m_ParallelNode;
        }
    }
}
