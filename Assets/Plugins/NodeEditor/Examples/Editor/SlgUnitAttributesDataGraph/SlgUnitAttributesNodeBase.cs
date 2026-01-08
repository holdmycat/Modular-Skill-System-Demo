using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    public class SlgUnitAttributesNodeBase : BaseNode
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        /// <summary>
        /// For compatibility with BaseGraphView inspector logic (expects NP_GetNodeDataBase).
        /// </summary>
        public virtual SlgUnitAttributesNodeData NP_GetNodeDataBase()
        {
            return SlgAttributesData_GetNodeData();
        }

        public virtual SlgUnitAttributesNodeData SlgAttributesData_GetNodeData()
        {
            return null;
        }
    }
}
