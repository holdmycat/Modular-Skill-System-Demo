using Ebonor.DataCtrl;
using GraphProcessor;

namespace Plugins.NodeEditor
{
    public class UnitAttributesNodeBase: BaseNode
    {
        public override bool isRenamable => true;
        public virtual UnitAttributesNodeDataBase UnitAttributesData_GetNodeData()
        {
            return null;
        }
    }
    
}