//------------------------------------------------------------
// File: ShieldInfantrySquadAttributesNodeData.cs
// Purpose: Default squad attributes for shield infantry.
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitSquadAttributesNodeData>))]
    public class ShieldInfantrySquadAttributesNodeData : SlgUnitSquadAttributesNodeData
    {
        // public ShieldInfantrySquadAttributesNodeData()
        // {
        //     UnitId = 0; // 需要在数据表中填入对应的 ShieldInfantry UnitId
        //     InitialCount = 10;
        //     MaxCount = 10;
        //     Formation = "Line";
        //     SpawnYaw = 0f;
        // }
    }
}
