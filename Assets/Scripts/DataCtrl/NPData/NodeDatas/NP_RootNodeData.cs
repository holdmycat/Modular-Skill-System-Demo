//------------------------------------------------------------
// File: NP_RootNodeData.cs
// Created: 2025-12-01
// Purpose: Root node data for behavior trees.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class NP_RootNodeData : NP_NodeDataBase
    {
        [HideInInspector]
        public Root m_Root;
        
        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            m_Root = new Root(node, runtimeTree.Clock);
            
            return m_Root;
        }
        
        public override Node NP_GetNode()
        {
            return this.m_Root;
        }
    }
}