using GraphProcessor;
using Ebonor.DataCtrl;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    public abstract class SlgUnitSquadAttributesNodeBase : BaseNode
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        /// <summary>
        /// For inspector compatibility with BaseGraphView (expects NP_GetNodeDataBase).
        /// </summary>
        public virtual SlgUnitSquadAttributesNodeData NP_GetNodeDataBase()
        {
            return SlgSquadAttributesData_GetNodeData();
        }

        public virtual SlgUnitSquadAttributesNodeData SlgSquadAttributesData_GetNodeData()
        {
            return null;
        }
    }
}
