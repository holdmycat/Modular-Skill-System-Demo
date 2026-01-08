using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/装饰/Repeater", typeof (SlgSquadBehavourGraph))]
    public class NP_RepeaterNode: NP_DecoratorNodeBase
    {
        public override string name => "重复执行结点";

        public NP_RepeaterNodeData NpRepeaterNodeData = new NP_RepeaterNodeData { NodeDes = "重复执行结点数据" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NpRepeaterNodeData;
        }
    }
}
