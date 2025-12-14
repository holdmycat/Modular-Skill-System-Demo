using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface ICharacterDataRepository
    {
        UniTask SaveUnitDataSupporterAsync(UnitAttributesDataSupportor supportor);

        UnitAttributesNodeDataBase GetUnitAttribteData(long unitId);

        UnitAttributesNodeDataBase GetUnitAttributeNodeDataByUnitName(string id);
        
        Dictionary<long, UnitAttributesNodeDataBase> GetAllUnitAttribteData();

    }
}

