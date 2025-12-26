using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable, NodeMenuItem("AttributeNode/Commander/Molly (Economy)", typeof(SlgCommanderAttributesDataGraph))]
    public class MollyCommanderNode : SlgCommanderAttributesNodeBase
    {
        public override string name => "Molly (Economy)";

        [SerializeReference]
        public MollyCommanderAttributesNodeData Data;

        protected override void Enable()
        {
            if (Data == null)
            {
                Data = new MollyCommanderAttributesNodeData();
                Data.CommanderName = "Molly";
                Data.Level = 10;
                // Economy Focus
                Data.GlobalConstructionSpeedMod = 200; // 20%
                Data.GlobalResearchSpeedMod = 150;     // 15%
            }
        }

        public override SlgCommanderAttributesNodeData CommanderAttributesData_GetNodeData()
        {
            return Data;
        }
    }
}
