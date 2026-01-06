using GraphProcessor;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/任务/Squad 状态机/清空栈动画状态", typeof(SlgSquadBehavourGraph))]
    public class NP_ClearSquadStackStateActionNode : NP_TaskNodeBase
    {
        public override string name => "清空栈动画状态";

        public NP_ActionNodeData NP_ActionNodeData = new NP_ActionNodeData
        {
            NpClassForStoreAction = new NP_ClearSquadStackStateAction(),
            NodeDes = "清空栈动画状态"
        };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }
}
