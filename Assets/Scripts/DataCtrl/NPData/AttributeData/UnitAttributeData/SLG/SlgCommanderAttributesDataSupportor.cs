//------------------------------------------------------------
// File: CommanderAttributesDataSupportor.cs
// Purpose: Container mapping commander attribute IDs to attribute data.
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    public class SlgCommanderAttributesDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, SlgCommanderAttributesNodeData> CommanderAttributesDic = new Dictionary<long, SlgCommanderAttributesNodeData>();
    }
}
