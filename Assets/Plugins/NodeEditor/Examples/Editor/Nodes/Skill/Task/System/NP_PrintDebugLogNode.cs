using GraphProcessor;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Task/System/打印日志", typeof(SlgSquadBehavourGraph))]
    public class NP_PrintDebugLogNode : NP_TaskNodeBase
    {
        public override string name => "打印日志";
        
        public NP_ActionNodeData NP_ActionNodeData =
            new NP_ActionNodeData()
            {
                NpClassForStoreAction = new NP_PrintDebugLog(),
                NodeDes = "打印日志"
            };
        
        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }
}
