using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    [NodeMenuItem("SLG 小队/组合/Selector", typeof (SlgSquadBehavourGraph))]
    public class NP_SelectorNode: NP_CompositeNodeBase
    {
        public override string name => "选择结点";

        public NP_SelectorNodeData NP_SelectorNodeData = new NP_SelectorNodeData { NodeDes = "选择组合器"};

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_SelectorNodeData;
        }
    }
}
