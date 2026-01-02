using System;
using System.Collections.Generic;
using Ebonor.Framework;
using Zenject;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public class ModelRepository : IModelRepository,IDisposable, IInitializable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelRepository));
        private Dictionary<long, UObject> _dicGameModel;
        private Dictionary<string, UObject> _dicModelName;
        private ICharacterDataRepository _characterDataRepository;
        
        public UObject GetGameModel(long id)
        {
            if(_dicGameModel.TryGetValue(id, out var obj))
            {
                return obj;
            }
            return null;
        }
        
        
        public void SaveModelAsync(IList<UObject> list)
        {
            log.Info("[ModelRepository], Starting save model");
            
            _dicGameModel.Clear();
            
            if (list == null)
            {
                log.Warn("[ModelRepository] Hero model prefabs not found; skipping model load.");
                return;
            }

            foreach (var variable in list)
            {
                _dicModelName.Add(variable.name, variable);
            }
            
            foreach (var variable in _characterDataRepository.GetAllSlgUnitAttribteData())
            {
                var modelId = variable.Key;

                var unitAttr = variable.Value;
                
                bool found = false;
                
                // Check exact match first
                if (_dicModelName.TryGetValue(unitAttr.UnitName, out var model))
                {
                    if (!_dicGameModel.ContainsKey(modelId))
                    {
                        _dicGameModel.Add(modelId, model);
                    }
                    found = true;
                }
                else
                {
                    // Check Faction Suffixes
                    foreach (FactionType faction in Enum.GetValues(typeof(FactionType)))
                    {
                        string suffixedName = $"{unitAttr.UnitName}_{faction}";
                        if (_dicModelName.TryGetValue(suffixedName, out var factionModel))
                        {
                            if (!_dicGameModel.ContainsKey(modelId))
                            {
                                _dicGameModel.Add(modelId, factionModel);
                            }
                            found = true;
                            // found at least one variant, mark as found. 
                            // We construct the map with *one* of them to avoid ID collision.
                        }
                    }
                }

                if (!found)
                {
                    // Just continue, no fatal error block.
                    // log.WarnFormat("[ModelRepository] UnitName:{0} not found (checked suffixes).", unitAttr.UnitName);
                    continue;
                }

                log.InfoFormat("[ModelRepository], modelId:{0}, ModelName:{1}, Found Valid Model", unitAttr.UnitDataNodeId, unitAttr.UnitName);
            }
            
            
        }

        public void Dispose()
        {
            log.Info("[ModelRepository], Starting Dispose");
            _dicGameModel.Clear();
            _dicModelName.Clear();
        }

        public void Initialize()
        {
            log.Info("[ModelRepository], Starting Initialize");
            _dicModelName.Clear();
            _dicGameModel.Clear();
           
        }
        
        public ModelRepository(ICharacterDataRepository characterDataRepository)
        {
            log.Info("[ModelRepository], Starting Construction");
            _characterDataRepository = characterDataRepository;
            _dicGameModel = new Dictionary<long, UObject>();
            _dicModelName = new Dictionary<string, UObject>();
            
        }
    }
}

