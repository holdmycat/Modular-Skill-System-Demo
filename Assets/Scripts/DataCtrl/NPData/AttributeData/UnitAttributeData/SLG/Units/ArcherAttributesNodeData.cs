//------------------------------------------------------------
// File: ArcherAttributesNodeData.cs
// Purpose: SLG ranged archer attributes.
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitAttributesNodeData>))]
    public class ArcherAttributesNodeData : SlgUnitAttributesNodeData
    {
        // public ArcherAttributesNodeData()
        // {
        //     UnitName = "Archer";
        //     CombatPositionType = CombatPositionType.Ranged;
        //     UnitClassType = UnitClassType.Archer;
        //     ArmorType = ArmorType.Light;
        //     DamageType = DamageType.Pierce;
        //
        //     BaseHp = 120f;
        //     Attack = 35f;
        //     AttackRange = 12f;
        //     MoveSpeed = 3.8f;
        //     PopulationCost = 1;
        //     SquadSize = 10;
        // }
    }
}
