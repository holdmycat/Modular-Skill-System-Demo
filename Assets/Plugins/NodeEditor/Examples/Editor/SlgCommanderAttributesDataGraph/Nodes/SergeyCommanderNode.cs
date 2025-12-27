using GraphProcessor;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Plugins.NodeEditor
{
    [System.Serializable, NodeMenuItem("AttributeNode/Commander/Sergey (Military)", typeof(SlgCommanderAttributesDataGraph))]
    public class SergeyCommanderNode : SlgCommanderAttributesNodeBase
    {
        public override string name => "Sergey (Military)";

        [SerializeReference]
        public SergeyCommanderAttributesNodeData Data;

        protected override void Enable()
        {
            if (Data == null)
            {
                Data = new SergeyCommanderAttributesNodeData();
                Data.UnitName = "Sergey";
                Data.Level = 10;
                // Military Focus
                Data.GlobalInfantryAtkMod = 300;     // 30%
                Data.GlobalMarchSpeedMod = 100;      // 10%
            }
        }

        public override SlgCommanderAttributesNodeData CommanderAttributesData_GetNodeData()
        {
            return Data;
        }
    }
}
