using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    [BsonDeserializerRegister]
    public abstract class NP_BaseDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string,long> Ids = new Dictionary<string, long>();

        // [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        // public Dictionary<long, BuffNodeDataBase> BuffNodeDataDic = new Dictionary<long, BuffNodeDataBase>();

        public virtual NP_BaseDataSupportor Clone()
        {
            var clone = (NP_BaseDataSupportor)MemberwiseClone();
            clone.Ids = new Dictionary<string, long>(Ids);
            return clone;
        }
    }
    
    /// <summary>
    /// 技能配置数据载体
    /// </summary>
    [System.Serializable]
    [BsonDeserializerRegister]
    public class NP_DataSupportor : NP_BaseDataSupportor
    {
        public NP_DataSupportorBase NpDataSupportorBase = new NP_DataSupportorBase();

        public override NP_BaseDataSupportor Clone()
        {
            var clone = (NP_DataSupportor)base.Clone();
            clone.NpDataSupportorBase = NpDataSupportorBase?.Clone();
            return clone;
        }
    }
}
