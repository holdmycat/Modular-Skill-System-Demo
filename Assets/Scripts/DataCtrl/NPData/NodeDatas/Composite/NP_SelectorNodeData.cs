//------------------------------------------------------------
// File: NP_SelectorNodeData.cs
// Created: 2025-12-01
// Purpose: Selector composite node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
    public class NP_SelectorNodeData:NP_NodeDataBase
    {
        private Selector m_SelectorNode;

        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_SelectorNode = new Selector(nodes);
            return this.m_SelectorNode;
        }

        public override Node NP_GetNode()
        {
            return this.m_SelectorNode;
        }
    }
}
