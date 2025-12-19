//------------------------------------------------------------
// File: ArcherSquadAttributesNode.cs
// Purpose: Graph node for archer squad attributes.
//------------------------------------------------------------
using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable]
    [NodeMenuItem("Unit Attributes/SLG Squad/Archer Squad", typeof(SlgUnitSquadAttributesDataGraph))]
    public class ArcherSquadAttributesNode : SlgUnitSquadAttributesNodeBase
    {
        public override bool isRenamable => true;
        public override bool needsInspector => true;

        [SerializeReference]
        private SlgUnitSquadAttributesNodeData data = new ArcherSquadAttributesNodeData();

        public override SlgUnitSquadAttributesNodeData SlgSquadAttributesData_GetNodeData()
        {
            return data;
        }
    }
}
