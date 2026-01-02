
namespace Ebonor.DataCtrl
{
    public class NP_RepeaterNodeData:NP_NodeDataBase
    {
        public Repeater m_Repeater;
        
        public int loopCount = -1;
        
        public override Node NP_GetNode()
        {
            return this.m_Repeater;
        }
        
        public override Decorator CreateDecoratorNode(uint unit, NP_RuntimeTree runtimeTree, Node node)
        {
            this.m_Repeater = new Repeater(loopCount, node);
            return this.m_Repeater;
        }
        
       

        public override Decorator CreateNGDecoratorNode<T>(string unit, T runtimeTree, Clock clock, Node node)
        {
            this.m_Repeater = new Repeater(node);
            return this.m_Repeater;
        }

    }
}
