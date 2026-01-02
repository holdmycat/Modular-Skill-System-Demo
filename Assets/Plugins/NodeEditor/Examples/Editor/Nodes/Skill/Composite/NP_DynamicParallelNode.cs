using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Composite/DynamicParallel", typeof (SlgSquadBehavourGraph))]
    public class NP_DynamicParallelNode: NP_CompositeNodeBase
    {
        public override string name => "动态并行节点";

        public NP_DynamicParallelNodeData NP_DynamicParallelNodeData = new NP_DynamicParallelNodeData { NodeDes = "动态并行组合器" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_DynamicParallelNodeData;
        }
        
    }
}
