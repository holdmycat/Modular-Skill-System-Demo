using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/组合/Sequence", typeof (SlgSquadBehavourGraph))]
    public class NP_SequenceNode: NP_CompositeNodeBase
    {
        public override string name => "序列结点";

        public NP_SequenceNodeData NP_SequenceNodeData = new NP_SequenceNodeData { NodeDes = "序列组合器"};

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_SequenceNodeData;
        }
    }
}
