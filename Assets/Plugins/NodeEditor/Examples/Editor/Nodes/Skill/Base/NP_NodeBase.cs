using GraphProcessor;
using UnityEngine;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    public abstract class NP_NodeBase : BaseNode
    {
        
        public override bool needsInspector => true;

        /// <summary>
        /// 层级，用于自动排版
        /// </summary>
        public int Level;
        
        public virtual NP_NodeDataBase NP_GetNodeData()
        {
            return null;
        }

        /// <summary>
        /// Exposed for the graph inspector: BaseGraphView reflects this method to populate "Current Data".
        /// </summary>
        public virtual NP_NodeDataBase NP_GetNodeDataBase()
        {
            return NP_GetNodeData();
        }
    }
}
