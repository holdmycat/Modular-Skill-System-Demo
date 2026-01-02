//------------------------------------------------------------
// File: NP_TimeRepeaterNodeData.cs
// Created: 2025-12-01
// Purpose: Time-based repeater decorator node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

namespace Ebonor.DataCtrl
{
    public class NP_TimeRepeaterNodeData:NP_NodeDataBase
    {
        public TimeRepeater m_Repeater;
        
         public float Interval;
        
        
        public override Node NP_GetNode()
        {
            return this.m_Repeater;
        }
        
        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            this.m_Repeater = new TimeRepeater(Interval, node);
            return this.m_Repeater;
        }
        
    }
}
