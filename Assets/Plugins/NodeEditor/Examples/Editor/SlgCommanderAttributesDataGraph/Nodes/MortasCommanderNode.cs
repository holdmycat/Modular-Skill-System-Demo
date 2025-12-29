using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable, NodeMenuItem("AttributeNode/Commander/Mortas (Boss)", typeof(SlgCommanderAttributesDataGraph))]
    public class MortasCommanderNode : SlgCommanderAttributesNodeBase
    {
        public override string name => "Mortas (Boss)";

        [SerializeReference]
        public MortasCommanderAttributesNodeData Data;

        protected override void Enable()
        {
            if (Data == null)
            {
                Data = new MortasCommanderAttributesNodeData();
                Data.UnitName = "Mortas";
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
