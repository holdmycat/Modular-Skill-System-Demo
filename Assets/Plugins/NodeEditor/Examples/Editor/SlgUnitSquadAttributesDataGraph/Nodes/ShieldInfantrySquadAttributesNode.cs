//------------------------------------------------------------
// File: ShieldInfantrySquadAttributesNode.cs
// Purpose: Graph node for shield infantry squad attributes.
//------------------------------------------------------------
using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    [NodeMenuItem("Unit Attributes/SLG Squad/Shield Infantry Squad", typeof(SlgUnitSquadAttributesDataGraph))]
    public class ShieldInfantrySquadAttributesNode : SlgUnitSquadAttributesNodeBase
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        [SerializeReference]
        private SlgUnitSquadAttributesNodeData data = new ShieldInfantrySquadAttributesNodeData();

        public override SlgUnitSquadAttributesNodeData SlgSquadAttributesData_GetNodeData()
        {
            return data;
        }
    }
}
