//------------------------------------------------------------
// Author: YanYuMiLiBanShiShang
// Mail: 1778139321@qq.com
// Date: 2019-07-25 19:20:42
//------------------------------------------------------------

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ebonor.DataCtrl
{
    public class UnitAttributesDataSupportor
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<uint, UnitAttributesNodeDataBase> UnitAttributesDataSupportorDic = new Dictionary<uint, UnitAttributesNodeDataBase>();
    }
}
