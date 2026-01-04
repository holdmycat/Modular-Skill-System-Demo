using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Task/NpBehave/修改黑板值", typeof(SlgSquadBehavourGraph))]
    public class NP_ChangeBlackValueActionNode : NP_TaskNodeBase
    {
        public override string name => "修改黑板值";

        public NP_ActionNodeData NP_ActionNodeData = new NP_ActionNodeData
        {
            NpClassForStoreAction = new NP_ChangeBlackValueAction(),
            NodeDes = "修改黑板值"
        };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }
}
