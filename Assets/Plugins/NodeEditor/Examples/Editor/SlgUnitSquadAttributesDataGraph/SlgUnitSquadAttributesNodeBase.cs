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
        /// For inspector compatibility with BaseGraphView (expects UnitAttributesData_GetNodeData).
        /// </summary>
        public virtual SlgUnitSquadAttributesNodeData UnitAttributesData_GetNodeData()
        {
            return SlgSquadAttributesData_GetNodeData();
        }

        public virtual SlgUnitSquadAttributesNodeData SlgSquadAttributesData_GetNodeData()
        {
            return null;
        }
    }
}
