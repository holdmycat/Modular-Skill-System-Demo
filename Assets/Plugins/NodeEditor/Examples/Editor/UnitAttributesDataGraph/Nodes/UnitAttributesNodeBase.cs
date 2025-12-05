using Ebonor.DataCtrl;
using GraphProcessor;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    public class UnitAttributesNodeBase: BaseNode
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;
        public virtual UnitAttributesNodeDataBase UnitAttributesData_GetNodeData()
        {
            return null;
        }
    }
    
}
