using GraphProcessor;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Plugins.NodeEditor
{
    
    [NodeMenuItem("SlgSquad行为树/Task/Time/等待行为节点", typeof (SlgSquadBehavourGraph))]
    public class NP_WaitActionNode : NP_TaskNodeBase
    {
        public override string name => "等待行为节点";
        
        public NP_ActionNodeData NP_ActionNodeData =
            new NP_ActionNodeData() { NpClassForStoreAction = new NP_WaitAction() };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ActionNodeData;
        }
    }

}
