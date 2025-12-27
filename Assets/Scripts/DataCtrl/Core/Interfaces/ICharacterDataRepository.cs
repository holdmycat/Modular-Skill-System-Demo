using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    public interface ICharacterDataRepository
    {
        UniTask SaveUnitDataSupporterAsync(UnitAttributesDataSupportor supportor);

        UniTask SaveSlgUnitDataSupporterAsync(SlgUnitAttributesDataSupportor dataSupportor);
        UniTask SaveSlgSquadDataSupporterAsync(SlgUnitSquadAttributesDataSupportor dataSupportor);
        UniTask SaveSlgCommanderDataSupporterAsync(SlgCommanderAttributesDataSupportor dataSupportor);
        
        UnitAttributesNodeDataBase GetUnitAttribteData(long unitId);

        UnitAttributesNodeDataBase GetUnitAttributeNodeDataByUnitName(string id);
        
        SlgUnitAttributesNodeData GetSlgUnitAttributeNodeDataByUnitName(string id);
        
        Dictionary<long, UnitAttributesNodeDataBase> GetAllUnitAttribteData();

        Dictionary<long, SlgUnitAttributesNodeData> GetAllSlgUnitAttribteData();
        
        SlgUnitAttributesNodeData GetSlgUnitData(long unitId);
        
        SlgUnitSquadAttributesNodeData GetSlgSquadData(long unitId);


        SlgCommanderAttributesNodeData GetSlgCommanderData(long unitId);

        SlgCommanderAttributesNodeData GetSlgCommanderDataBySquadName(string unitId);

        Dictionary<long, SlgCommanderAttributesNodeData> GetAllSlgCommanderAttribteData();


    }
}

