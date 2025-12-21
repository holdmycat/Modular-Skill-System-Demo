using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface ICharacterDataRepository
    {
        UniTask SaveUnitDataSupporterAsync(UnitAttributesDataSupportor supportor);

        UniTask SaveSlgUnitDataSupporterAsync(SlgUnitAttributesDataSupportor dataSupportor);
        UniTask SaveSlgSquadDataSupporterAsync(SlgUnitSquadAttributesDataSupportor dataSupportor);
        
        UnitAttributesNodeDataBase GetUnitAttribteData(long unitId);

        UnitAttributesNodeDataBase GetUnitAttributeNodeDataByUnitName(string id);
        
        Dictionary<long, UnitAttributesNodeDataBase> GetAllUnitAttribteData();

        SlgUnitAttributesNodeData GetSlgUnitData(long unitId);
        
        SlgUnitSquadAttributesNodeData GetSlgSquadData(long unitId);

    }
}

