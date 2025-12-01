//------------------------------------------------------------
// File: NP_DataSupportor.cs
// Created: 2025-12-01
// Purpose: Behavior tree data support container for skills.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    [BsonDeserializerRegister]
    public abstract class NP_BaseDataSupportor
    {
        [Tooltip("Skill-related ID mappings.")]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string,long> Ids = new Dictionary<string, long>();

        [Tooltip("Buff node data within the skill.")]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, BuffNodeDataBase> BuffNodeDataDic = new Dictionary<long, BuffNodeDataBase>();

    }
    
    /// <summary>
    /// Skill configuration data container.
    /// </summary>
    [BsonDeserializerRegister]
    public class NP_DataSupportor : NP_BaseDataSupportor
    {
        public NP_DataSupportorBase NpDataSupportorBase = new NP_DataSupportorBase();
        
    }
}
