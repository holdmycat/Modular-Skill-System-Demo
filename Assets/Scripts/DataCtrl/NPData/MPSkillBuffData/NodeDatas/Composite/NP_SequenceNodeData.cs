
namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_SequenceNodeData: NP_NodeDataBase
    {
        private Sequence m_SequenceNode;

        public override Node NP_GetNode()
        {
            return this.m_SequenceNode;
        }

        public override Composite CreateComposite(Node[] nodes)
        {
            this.m_SequenceNode = new Sequence(nodes);
            return this.m_SequenceNode;
        }
    }
}
