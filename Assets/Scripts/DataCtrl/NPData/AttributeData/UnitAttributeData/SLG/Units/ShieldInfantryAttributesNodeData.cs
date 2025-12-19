//------------------------------------------------------------
// File: ShieldInfantryAttributesNodeData.cs
// Purpose: SLG melee shield infantry attributes.
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitAttributesNodeData>))]
    public class ShieldInfantryAttributesNodeData : SlgUnitAttributesNodeData
    {
        // public ShieldInfantryAttributesNodeData()
        // {
        //     UnitName = "ShieldInfantry";
        //     CombatPositionType = CombatPositionType.Melee;
        //     UnitClassType = UnitClassType.Infantry;
        //     ArmorType = ArmorType.Heavy;
        //     DamageType = DamageType.Blunt;
        //
        //     BaseHp = 200f;
        //     Attack = 25f;
        //     AttackRange = 1.5f;
        //     MoveSpeed = 3.5f;
        //     PopulationCost = 1;
        //     SquadSize = 10;
        // }
    }
}
