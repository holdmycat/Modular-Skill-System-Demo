
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_DataSupportorBase
    {
        public long NPBehaveTreeDataId;

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<long, NP_NodeDataBase> NP_DataSupportorDic = new Dictionary<long, NP_NodeDataBase>();
        
        public Dictionary<string, ANP_BBValue> NP_BBValueManager = new Dictionary<string, ANP_BBValue>();

        public NP_DataSupportorBase Clone()
        {
            var clone = (NP_DataSupportorBase)MemberwiseClone();
            clone.NP_DataSupportorDic = new Dictionary<long, NP_NodeDataBase>(NP_DataSupportorDic.Count);
            foreach (var kvp in NP_DataSupportorDic)
            {
                var nodeCopy = kvp.Value?.Clone();
                if (nodeCopy != null)
                {
                    clone.NP_DataSupportorDic[kvp.Key] = nodeCopy;
                }
            }

            clone.NP_BBValueManager = new Dictionary<string, ANP_BBValue>(NP_BBValueManager.Count);
            foreach (var kvp in NP_BBValueManager)
            {
                var bbCopy = kvp.Value?.Clone();
                if (bbCopy != null)
                {
                    clone.NP_BBValueManager[kvp.Key] = bbCopy;
                }
            }

            return clone;
        }
        
    }
    
}
