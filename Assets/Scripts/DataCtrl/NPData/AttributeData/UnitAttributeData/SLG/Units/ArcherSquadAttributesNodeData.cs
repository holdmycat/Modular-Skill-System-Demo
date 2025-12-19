//------------------------------------------------------------
// File: ArcherSquadAttributesNodeData.cs
// Purpose: Default squad attributes for archers.
//------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonSerializer(typeof(AttributesDataSerializer<SlgUnitSquadAttributesNodeData>))]
    public class ArcherSquadAttributesNodeData : SlgUnitSquadAttributesNodeData
    {
        // public ArcherSquadAttributesNodeData()
        // {
        //     UnitId = 0; // 需要在数据表中填入对应的 Archer UnitId
        //     InitialCount = 10;
        //     MaxCount = 10;
        //     Formation = "Loose";
        //     SpawnYaw = 0f;
        // }
    }
}
