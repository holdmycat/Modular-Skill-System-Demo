
namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_BlackboardConditionNodeData: NP_NodeDataBase
    {
        private BlackboardCondition m_BlackboardConditionNode;

        public Operator Ope = Operator.IS_EQUAL;

        public Stops Stop = Stops.IMMEDIATE_RESTART;

        public NP_BlackBoardRelationData NPBalckBoardRelationData = new NP_BlackBoardRelationData() { WriteOrCompareToBB = true };

        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            this.m_BlackboardConditionNode = new BlackboardCondition(this.NPBalckBoardRelationData.BBKey,
                this.Ope,
                this.NPBalckBoardRelationData.NP_BBValue, this.Stop, node);
            //此处的value参数可以随便设，因为我们在游戏中这个value是需要动态改变的
            return this.m_BlackboardConditionNode;
        }

        public override Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node)
        {
            this.m_BlackboardConditionNode = new BlackboardCondition(this.NPBalckBoardRelationData.BBKey,
                this.Ope,
                this.NPBalckBoardRelationData.NP_BBValue, this.Stop, node);
            //此处的value参数可以随便设，因为我们在游戏中这个value是需要动态改变的
            return this.m_BlackboardConditionNode;
        }
        
        public override Node NP_GetNode()
        {
            return this.m_BlackboardConditionNode;
        }
    }
}
