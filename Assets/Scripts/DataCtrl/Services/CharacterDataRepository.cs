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
        
        [Obsolete]
        private Dictionary<long, UnitAttributesNodeDataBase> _dicUnitAttriDatas;
        [Obsolete]
        private Dictionary<string, UnitAttributesNodeDataBase> _dicUnitAttriDatasByName;
        [Obsolete]
        private Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>> _dicListUnitDatas;

        private Dictionary<long, SlgUnitAttributesNodeData> _dicSlgUnitAttriDatas;
        private Dictionary<string, SlgUnitAttributesNodeData> _dicSlgUnitAttriDatasByName;
        
        // SLG Squad Data
        private Dictionary<long, SlgUnitSquadAttributesNodeData> _dicSlgSquadAttriDatas;
        private Dictionary<string, SlgUnitSquadAttributesNodeData> _dicSlgSquadAttriDatasByName;
        
        // SLG Commander Data
        private Dictionary<long, SlgCommanderAttributesNodeData> _dicCommanderAttriDatas;
        private Dictionary<string, SlgCommanderAttributesNodeData> _dicCommanderAttriDatasByName;
        
        public CharacterDataRepository()
        {
            
            log.Info("[CharacterDataRepository], Starting Construction");
            _dicSlgUnitAttriDatasByName = new Dictionary<string, SlgUnitAttributesNodeData>();
            _dicSlgUnitAttriDatas = new Dictionary<long, SlgUnitAttributesNodeData>();
            
            _dicSlgSquadAttriDatas = new Dictionary<long, SlgUnitSquadAttributesNodeData>();
            _dicSlgSquadAttriDatasByName = new Dictionary<string, SlgUnitSquadAttributesNodeData>();
            
            _dicCommanderAttriDatas = new Dictionary<long, SlgCommanderAttributesNodeData>();
            _dicCommanderAttriDatasByName = new Dictionary<string, SlgCommanderAttributesNodeData>();
            
            
            _dicUnitAttriDatas = new Dictionary<long, UnitAttributesNodeDataBase>();
            _dicUnitAttriDatasByName = new Dictionary<string, UnitAttributesNodeDataBase>();
            _dicListUnitDatas = new Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>>();
            for (var i = eActorModelType.eHero; i < eActorModelType.eSize; i++)
            {
                _dicListUnitDatas.Add(i, new List<UnitAttributesNodeDataBase>());
            }
        }
        
        [Obsolete]
        public async UniTask SaveUnitDataSupporterAsync(UnitAttributesDataSupportor dataSupportor)
        {
            log.Info("[CharacterDataRepository], Starting save unit data");
            
            foreach (var hero in dataSupportor.UnitAttributesDataSupportorDic.Values)  
            {                    
                _dicUnitAttriDatas.Add(hero.UnitDataNodeId, hero);  
                _dicListUnitDatas[hero.ActorModelType].Add(hero);  
                _dicUnitAttriDatasByName.Add(hero.UnitName, hero);
                log.Info("[CharacterDataRepository] Saving unit data heroId:" + hero.UnitDataNodeId);
            }
            
            await UniTask.CompletedTask;
        }
        public async UniTask SaveSlgUnitDataSupporterAsync(SlgUnitAttributesDataSupportor dataSupportor)
        {
            log.Info("[CharacterDataRepository] Starting save SLG unit data");
            
            foreach (var unit in dataSupportor.SlgUnitAttributesDic.Values)
            {
                // Ensure ID uniqueness or handle duplicates if necessary, but Dictionary.Add throws on dup.
                // Assuming SLG units have unique IDs distinct from Heroes.
                if (!_dicSlgUnitAttriDatas.ContainsKey(unit.UnitDataNodeId))
                {
                    _dicSlgUnitAttriDatas.Add(unit.UnitDataNodeId, unit);
                    
                    if (!_dicSlgUnitAttriDatasByName.ContainsKey(unit.UnitName))
                    {
                        _dicSlgUnitAttriDatasByName.Add(unit.UnitName, unit);
                    }
                    
                    log.Info("[CharacterDataRepository] Saving SLG unit data id:" + unit.UnitDataNodeId);
                }
                else
                {
                    log.Warn($"[CharacterDataRepository] Duplicate Unit ID found: {unit.UnitDataNodeId}");
                }
            }

            await UniTask.CompletedTask;
        }
        public async  UniTask SaveSlgSquadDataSupporterAsync(SlgUnitSquadAttributesDataSupportor dataSupportor)
        {
            log.Info("[CharacterDataRepository] Starting save SLG Squad data");
            
            foreach (var squad in dataSupportor.SquadAttributesDic.Values)
            {
                if (!_dicSlgSquadAttriDatas.ContainsKey(squad.UnitDataNodeId))
                {
                    _dicSlgSquadAttriDatas.Add(squad.UnitDataNodeId, squad);
                    
                    if (!_dicSlgSquadAttriDatasByName.ContainsKey(squad.UnitName))
                    {
                        _dicSlgSquadAttriDatasByName.Add(squad.UnitName, squad);
                    }
                    
                    log.Info("[CharacterDataRepository] Saving SLG Squad data id:" + squad.UnitDataNodeId);
                }
                else
                {
                    log.Warn($"[CharacterDataRepository] Duplicate Squad ID found: {squad.UnitDataNodeId}");
                }
            }

            await UniTask.CompletedTask;
        }
        public async UniTask SaveSlgCommanderDataSupporterAsync(SlgCommanderAttributesDataSupportor dataSupportor)
        {
            log.Info("[CharacterDataRepository] Starting save SLG Commander data");
            
            foreach (var squad in dataSupportor.CommanderAttributesDic.Values)
            {
                if (!_dicCommanderAttriDatas.ContainsKey(squad.UnitDataNodeId))
                {
                    _dicCommanderAttriDatas.Add(squad.UnitDataNodeId, squad);
                    
                    if (!_dicCommanderAttriDatasByName.ContainsKey(squad.UnitName))
                    {
                        _dicCommanderAttriDatasByName.Add(squad.UnitName, squad);
                    }
                    
                    log.Info("[CharacterDataRepository] Saving SLG Commander data id:" + squad.UnitDataNodeId);
                }
                else
                {
                    log.Warn($"[CharacterDataRepository] Duplicate Commander ID found: {squad.UnitDataNodeId}");
                }
            }

            await UniTask.CompletedTask;
        }
        
        //[Obsolete] unit attribute
        [Obsolete]
        public UnitAttributesNodeDataBase GetUnitAttribteData(long unitId)
        {
            if (_dicUnitAttriDatas.TryGetValue(unitId
                    , out var attr))
            {
                return attr;
            }

            return null;
        }
        [Obsolete]
        public UnitAttributesNodeDataBase GetUnitAttributeNodeDataByUnitName(string id)
        {
            if (_dicUnitAttriDatasByName.TryGetValue(id, out var attr))
            {
                return attr;
            }

            return null;
        }
        [Obsolete]
        public Dictionary<long, UnitAttributesNodeDataBase> GetAllUnitAttribteData()
        {
            return _dicUnitAttriDatas;
        }
        
        //slg unit
        public SlgUnitAttributesNodeData GetSlgUnitData(long unitId)
        {
            if (_dicSlgUnitAttriDatas.TryGetValue(unitId, out var attr))
            {
                return attr;
            }
            return null;
        }
        public Dictionary<long, SlgUnitAttributesNodeData> GetAllSlgUnitAttribteData()
        {
            return _dicSlgUnitAttriDatas;
        }
        public SlgUnitAttributesNodeData GetSlgUnitAttributeNodeDataByUnitName(string id)
        {
            if (_dicSlgUnitAttriDatasByName.TryGetValue(id, out var attr))
            {
                return attr;
            }

            return null;
        }
        
        //slg squad
        public SlgUnitSquadAttributesNodeData GetSlgSquadData(long unitId)
        {
            if (_dicSlgSquadAttriDatas.TryGetValue(unitId, out var attr))
            {
                return attr;
            }
            return null;
        }
        public SlgUnitSquadAttributesNodeData GetSlgSquadDataBySquadName(string unitId)
        {
            if (_dicSlgSquadAttriDatasByName.TryGetValue(unitId, out var attr))
            {
                return attr;
            }
            return null;
        }
        public Dictionary<long, SlgUnitSquadAttributesNodeData> GetAllSlgSquadAttribteData()
        {
            return _dicSlgSquadAttriDatas;
        }
        

        //slg commander
        public SlgCommanderAttributesNodeData GetSlgCommanderData(long unitId)
        {
            if (_dicCommanderAttriDatas.TryGetValue(unitId, out var attr))
            {
                return attr;
            }
            return null;
        }
        public SlgCommanderAttributesNodeData GetSlgCommanderDataBySquadName(string unitId)
        {
            if (_dicCommanderAttriDatasByName.TryGetValue(unitId, out var attr))
            {
                return attr;
            }
            return null;
        }
        public Dictionary<long, SlgCommanderAttributesNodeData> GetAllSlgCommanderAttribteData()
        {
            return _dicCommanderAttriDatas;
        } 
        
        
        public void Dispose()
        {
            log.Info("[CharacterDataRepository], Starting Dispose");
            _dicUnitAttriDatas.Clear();
            _dicUnitAttriDatasByName.Clear();
            _dicListUnitDatas.Clear();
            
            
            _dicSlgUnitAttriDatas.Clear();
            _dicSlgUnitAttriDatasByName.Clear();
            
            _dicSlgSquadAttriDatas.Clear();
            _dicSlgSquadAttriDatasByName.Clear();
            
            _dicCommanderAttriDatas.Clear();
            _dicCommanderAttriDatasByName.Clear();
        }

        public void Initialize()
        {
            log.Info("[CharacterDataRepository], Starting Initialize");
        }
    }

}
