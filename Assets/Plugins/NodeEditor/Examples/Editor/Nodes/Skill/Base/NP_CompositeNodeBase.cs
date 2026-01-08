using GraphProcessor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public abstract class NP_CompositeNodeBase : NP_NodeBase
    {
        public override Color color => new Color(0.5f, 0.2f, 0.8f);

        [Input("NPBehave_PreNode"), Vertical]
        [HideInInspector]
        public NP_NodeBase PrevNode;

        [Output("NPBehave_NextNode"), Vertical]
        [HideInInspector]
        public NP_NodeBase NextNode;
    }
}
