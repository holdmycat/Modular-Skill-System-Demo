//------------------------------------------------------------
// File: ShieldInfantryAttributesNode.cs
// Purpose: Graph node for melee shield infantry attributes.
//------------------------------------------------------------
using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    [NodeMenuItem("Unit Attributes/SLG/Melee Shield Infantry", typeof(SlgUnitAttributesDataGraph))]
    public class ShieldInfantryAttributesNode : SlgUnitAttributesNodeBase
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        [SerializeReference]
        private ShieldInfantryAttributesNodeData data = new ShieldInfantryAttributesNodeData();

        public override SlgUnitAttributesNodeData SlgAttributesData_GetNodeData()
        {
            return data;
        }
    }
}
