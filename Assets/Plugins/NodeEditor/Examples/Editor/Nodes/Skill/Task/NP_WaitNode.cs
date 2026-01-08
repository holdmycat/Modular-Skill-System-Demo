using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/任务/时间/Wait", typeof (SlgSquadBehavourGraph))]
    public class NP_WaitNode: NP_TaskNodeBase
    {
        public override string name => "等待节点";

        public NP_WaitNodeData NP_WaitNodeData = new NP_WaitNodeData
        {
            NodeDes = "等待节点",
        };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_WaitNodeData;
        }
    }
}
