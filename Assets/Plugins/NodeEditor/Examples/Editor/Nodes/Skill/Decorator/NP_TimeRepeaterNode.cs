using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SlgSquad行为树/Decorator/定时重复执行结点", typeof (SlgSquadBehavourGraph))]
    public class NP_TimeRepeaterNode: NP_DecoratorNodeBase
    {
        public override string name => "定时重复执行结点";

        public NP_TimeRepeaterNodeData NpRepeaterNodeData = new NP_TimeRepeaterNodeData { NodeDes = "定时重复执行结点" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NpRepeaterNodeData;
        }
    }
}
