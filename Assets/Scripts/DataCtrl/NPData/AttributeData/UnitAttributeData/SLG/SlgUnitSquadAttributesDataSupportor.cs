//------------------------------------------------------------
// File: SlgUnitSquadAttributesDataSupportor.cs
// Purpose: Container mapping squad attribute IDs to squad attribute data for SLG.
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    public class SlgUnitSquadAttributesDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, SlgUnitSquadAttributesNodeData> SquadAttributesDic = new Dictionary<long, SlgUnitSquadAttributesNodeData>();
    }
}
