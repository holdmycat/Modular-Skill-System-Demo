//------------------------------------------------------------
// File: UnitAttributesDataSupportor.cs
// Created: 2025-11-29
// Purpose: Container mapping unit IDs to attribute node data for serialization.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Container for unit attribute nodes keyed by unit id; used during graph export/import.
    /// </summary>
    public class UnitAttributesDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<uint, UnitAttributesNodeDataBase> UnitAttributesDataSupportorDic = new Dictionary<uint, UnitAttributesNodeDataBase>();
    }
}