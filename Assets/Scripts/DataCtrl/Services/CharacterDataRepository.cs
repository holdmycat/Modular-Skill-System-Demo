using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.DataCtrl
{
    public class CharacterDataRepository : ICharacterDataRepository,IDisposable, IInitializable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(CharacterDataRepository));
        
        private Dictionary<long, UnitAttributesNodeDataBase> _dicUnitAttriDatas;
        private Dictionary<string, UnitAttributesNodeDataBase> _dicUnitAttriDatasByName;
        private Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>> _dicListUnitDatas;

        public CharacterDataRepository()
        {
            
            log.Debug("[CharacterDataRepository], Starting Construction");
            
            _dicUnitAttriDatas = new Dictionary<long, UnitAttributesNodeDataBase>();
            _dicUnitAttriDatasByName = new Dictionary<string, UnitAttributesNodeDataBase>();
            _dicListUnitDatas = new Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>>();
            for (var i = eActorModelType.eHero; i < eActorModelType.eSize; i++)
            {
                _dicListUnitDatas.Add(i, new List<UnitAttributesNodeDataBase>());
            }
        }
        
        public async UniTask SaveUnitDataSupporterAsync(UnitAttributesDataSupportor dataSupportor)
        {
            log.Debug("[CharacterDataRepository], Starting save unit data");
            
            foreach (var hero in dataSupportor.UnitAttributesDataSupportorDic.Values)  
            {                    
                _dicUnitAttriDatas.Add(hero.UnitDataNodeId, hero);  
                _dicListUnitDatas[hero.ActorModelType].Add(hero);  
                _dicUnitAttriDatasByName.Add(hero.UnitName, hero);
                log.Debug("[CharacterDataRepository] Saving unit data heroId:" + hero.UnitDataNodeId);
            }
            
            await UniTask.CompletedTask;
        }

        public UnitAttributesNodeDataBase GetUnitAttribteData(long unitId)
        {
            if (_dicUnitAttriDatas.TryGetValue(unitId
                    , out var attr))
            {
                return attr;
            }

            return null;
        }
        
        public UnitAttributesNodeDataBase GetUnitAttributeNodeDataByUnitName(string id)
        {
            if (_dicUnitAttriDatasByName.TryGetValue(id, out var attr))
            {
                return attr;
            }

            return null;
        }

        public Dictionary<long, UnitAttributesNodeDataBase> GetAllUnitAttribteData()
        {
            return _dicUnitAttriDatas;
        }
        

        public void Dispose()
        {
            log.Debug("[CharacterDataRepository], Starting Dispose");
            _dicUnitAttriDatas.Clear();
            _dicUnitAttriDatasByName.Clear();
            _dicListUnitDatas.Clear();
            
        }

        public void Initialize()
        {
            log.Debug("[CharacterDataRepository], Starting Initialize");
        }
    }

}
