//------------------------------------------------------------
// File: NP_DataSupportorBase.cs
// Created: 2025-12-01
// Purpose: Base container for behavior tree nodes and blackboard data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    public class NP_DataSupportorBase
    {
        [Tooltip("Behavior tree ID (also the root node ID).")]
        public long NPBehaveTreeDataId;

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, NP_NodeDataBase> NP_DataSupportorDic = new Dictionary<long, NP_NodeDataBase>();
        
        [Tooltip("Blackboard key/value data.")]
        public Dictionary<string, ANP_BBValue> NP_BBValueManager = new Dictionary<string, ANP_BBValue>();
        
       
    }
    
}
