using GraphProcessor;
using Ebonor.DataCtrl;
namespace Plugins.NodeEditor
{
    
    [NodeMenuItem("SLG 小队/装饰/Service", typeof (SlgSquadBehavourGraph))]
    public class NP_ServiceNode: NP_DecoratorNodeBase
    {
        public override string name => "服务结点";

        public NP_ServiceNodeData NP_ServiceNodeData = new NP_ServiceNodeData { };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ServiceNodeData;
        }
    }
}
