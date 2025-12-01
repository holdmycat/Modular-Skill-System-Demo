//------------------------------------------------------------
// File: NP_DynamicParallelNodeData.cs
// Created: 2025-12-01
// Purpose: Dynamic parallel composite node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using UnityEngine;

namespace Ebonor.DataCtrl
{
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
