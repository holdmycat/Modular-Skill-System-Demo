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
        private Dictionary<eActorModelType, Dictionary<long, UObject>> _dicGameModel;
        private Dictionary<string, UObject> _dicModelName;
        private ICharacterDataRepository _characterDataRepository;
        
        public Dictionary<long, UObject> GetGameModelDic(eActorModelType type)
        {
            if(_dicGameModel.TryGetValue(type, out var dic))
            {
                return dic;
            }
            return null;
        }
        
        
        public void SaveModelAsync(IList<UObject> list)
        {
            log.Debug("[ModelRepository], Starting save model");
            
            if (list == null)
            {
                log.Warn("[ModelRepository] Hero model prefabs not found; skipping model load.");
                return;
            }

            foreach (var variable in list)
            {
                _dicModelName.Add(variable.name, variable);
            }
            
            foreach (var variable in _characterDataRepository.GetAllUnitAttribteData())
            {
                var modelId = variable.Key;

                var unitAttr = variable.Value;

                if (!_dicGameModel.TryGetValue(unitAttr.ActorModelType, out var dictionary))
                {
                    log.ErrorFormat("[ModelRepository] Fatal error, actorModel:{0} not exist", unitAttr.ActorModelType);
                    return;
                }

                if (!_dicModelName.TryGetValue(unitAttr.UnitName, out var model))
                {
                    log.ErrorFormat("[ModelRepository] Fatal error, unitName:{0} not exist", unitAttr.UnitName);
                    return;
                }
                
                dictionary.Add(modelId, model);

                _dicGameModel[unitAttr.ActorModelType] = dictionary;
                
                log.DebugFormat("[ModelRepository], actorType:{0}, modelId:{1}, ModelName:{2}", unitAttr.ActorModelType, unitAttr.UnitDataNodeId, unitAttr.UnitName);
                
            }
            
            
        }

        public void Dispose()
        {
            log.Debug("[ModelRepository], Starting Dispose");
            _dicGameModel.Clear();
            _dicModelName.Clear();
        }

        public void Initialize()
        {
            log.Debug("[ModelRepository], Starting Initialize");
            _dicModelName.Clear();
            _dicGameModel.Clear();
            for (var i = eActorModelType.eHero; i < eActorModelType.eSize; i++)
            {
                _dicGameModel.Add(i, new Dictionary<long, UObject>());
            }
        }
        
        public ModelRepository(ICharacterDataRepository characterDataRepository)
        {
            log.Debug("[ModelRepository], Starting Construction");
            _characterDataRepository = characterDataRepository;
            _dicGameModel = new Dictionary<eActorModelType, Dictionary<long, UObject>>();
            _dicModelName = new Dictionary<string, UObject>();
            
        }
    }
}

