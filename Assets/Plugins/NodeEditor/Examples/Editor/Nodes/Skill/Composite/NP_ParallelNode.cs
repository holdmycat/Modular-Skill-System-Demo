using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Composite/Parallel", typeof (SlgSquadBehavourGraph))]
    public class NP_ParallelNode: NP_CompositeNodeBase
    {
        public override string name => "并行节点";

        public NP_ParallelNodeData NP_ParallelNodeData = new NP_ParallelNodeData { NodeDes = "并行组合器" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ParallelNodeData;
        }
    }
}
