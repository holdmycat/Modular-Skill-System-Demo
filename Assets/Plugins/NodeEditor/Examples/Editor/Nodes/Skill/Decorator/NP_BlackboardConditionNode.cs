
using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Decorator/BlackboardCondition", typeof (SlgSquadBehavourGraph))]
    public class NP_BlackboardConditionNode: NP_DecoratorNodeBase
    {
        public override string name => "黑板条件结点";

        public NP_BlackboardConditionNodeData NP_BlackboardConditionNodeData =
                new NP_BlackboardConditionNodeData { NodeDes = "黑板条件结点" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_BlackboardConditionNodeData;
        }
    }
}
