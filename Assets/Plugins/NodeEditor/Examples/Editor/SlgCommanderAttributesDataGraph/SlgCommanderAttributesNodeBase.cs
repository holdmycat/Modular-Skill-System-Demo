using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    public abstract class SlgCommanderAttributesNodeBase : BaseNode
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        /// <summary>
        /// For inspector compatibility with existing graph inspector helpers (expects UnitAttributesData_GetNodeData).
        /// </summary>
        public virtual SlgCommanderAttributesNodeData UnitAttributesData_GetNodeData()
        {
            return CommanderAttributesData_GetNodeData();
        }

        public virtual SlgCommanderAttributesNodeData CommanderAttributesData_GetNodeData()
        {
            return null;
        }
    }
}
