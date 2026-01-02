
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

        public override Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node)
        {
            this.m_Repeater = new TimeRepeater(Interval, node);
            return this.m_Repeater;
        }

    }
}
