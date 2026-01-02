//------------------------------------------------------------
// File: NP_BlackboardConditionNodeData.cs
// Created: 2025-12-01
// Purpose: Blackboard condition decorator node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
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
            // value ， value 
            return this.m_BlackboardConditionNode;
        }

       
        
        public override Node NP_GetNode()
        {
            return this.m_BlackboardConditionNode;
        }
    }
}
