//------------------------------------------------------------
// File: SlgUnitAttributesDataSupportor.cs
// Purpose: Container mapping SLG unit IDs to SLG attribute node data.
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    public class SlgUnitAttributesDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, SlgUnitAttributesNodeData> SlgUnitAttributesDic = new Dictionary<long, SlgUnitAttributesNodeData>();
    }
}
