using GraphProcessor;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/任务/Squad 状态机/切换 Squad 栈状态", typeof(SlgSquadBehavourGraph))]
    public class NP_ChangeSquadStackStateActionNode : NP_TaskNodeBase
    {
        public override string name => "切换 Squad 栈状态";

        public NP_ActionNodeData NP_ActionNodeData = new NP_ActionNodeData
        {
            NpClassForStoreAction = new NP_ChangeSquadStackStateAction(),
            NodeDes = "切换 Squad 栈状态"
        };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }
}
