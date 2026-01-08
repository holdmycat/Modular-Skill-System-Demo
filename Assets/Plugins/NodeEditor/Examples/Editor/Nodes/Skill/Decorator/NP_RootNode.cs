using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{

    [NodeMenuItem("SLG 小队/根结点", typeof (SlgSquadBehavourGraph))]
    public class NP_RootNode: NP_NodeBase
    {
        public override string name => "行为树根节点";

        public override Color color => Color.green;

        [Output("NPBehave_NextNode"), Vertical]
        [HideInInspector]
        public NP_NodeBase NextNode;

        public NP_RootNodeData MRootNodeData = new NP_RootNodeData { };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return this.MRootNodeData;
        }
    }
}
