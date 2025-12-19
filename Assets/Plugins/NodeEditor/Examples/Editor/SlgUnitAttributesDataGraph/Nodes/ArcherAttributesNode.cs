//------------------------------------------------------------
// File: ArcherAttributesNode.cs
// Purpose: Graph node for ranged archer attributes.
//------------------------------------------------------------
using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    [NodeMenuItem("Unit Attributes/SLG/Ranged Archer", typeof(SlgUnitAttributesDataGraph))]
    public class ArcherAttributesNode : SlgUnitAttributesNodeBase
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        [SerializeReference]
        private ArcherAttributesNodeData data = new ArcherAttributesNodeData();

        public override SlgUnitAttributesNodeData SlgAttributesData_GetNodeData()
        {
            return data;
        }
    }
}
