using Ebonor.DataCtrl;
using GraphProcessor;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Task/NpBehave/等待直到收到停止指令", typeof (SlgSquadBehavourGraph))]
    public class NP_WaitUntilStoppedNode: NP_TaskNodeBase
    {
        public override string name => "一直等待，直到Stopped";

        public NP_WaitUntilStoppedData NpWaitUntilStoppedData = new NP_WaitUntilStoppedData { NodeDes = "阻止轮询，提高效率" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NpWaitUntilStoppedData;
        }
    }
}
