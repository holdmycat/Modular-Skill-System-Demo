
namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_DynamicParallelNodeData: NP_NodeDataBase
    {
        
        private DynamicParallel m_DynamicParallelNode;

        public Parallel.Policy SuccessPolicy = Parallel.Policy.ALL;

        public Parallel.Policy FailurePolicy = Parallel.Policy.ALL;

        public eSkillEventNode SkillEventNode;
        
        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_DynamicParallelNode = new DynamicParallel(SuccessPolicy, FailurePolicy, SkillEventNode, nodes);
            return this.m_DynamicParallelNode;
        }

        public override Node NP_GetNode()
        {
            return this.m_DynamicParallelNode;
        }
    }
}
