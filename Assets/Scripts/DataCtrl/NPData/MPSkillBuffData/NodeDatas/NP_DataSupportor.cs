using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    [BsonDeserializerRegister]
    public abstract class NP_BaseDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string,long> Ids = new Dictionary<string, long>();

        // [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        // public Dictionary<long, BuffNodeDataBase> BuffNodeDataDic = new Dictionary<long, BuffNodeDataBase>();

    }
    
    /// <summary>
    /// 技能配置数据载体
    /// </summary>
    [BsonDeserializerRegister]
    public class NP_DataSupportor : NP_BaseDataSupportor
    {
        public NP_DataSupportorBase NpDataSupportorBase = new NP_DataSupportorBase();
    }
}
