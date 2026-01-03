using System.Collections.Generic;

using UnityEngine;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_RootNodeData : NP_NodeDataBase
    {
        public Root m_Root;
        
        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            m_Root = new Root(node, runtimeTree.Clock);
            
            return m_Root;
        }

        public override Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node)
        {
            m_Root = new Root(node, clock);
            
            return m_Root;
        }

        public override Node NP_GetNode()
        {
            return this.m_Root;
        }
    }
}
