using GraphProcessor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public abstract class NP_TaskNodeBase : NP_NodeBase
    {
        [Input(" "), Vertical]
        [HideInInspector]
        public NP_NodeBase PrevNode;
    }
}
