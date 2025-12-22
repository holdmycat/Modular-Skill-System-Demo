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
            log.Debug("[ModelRepository], Starting save model");
            
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
                
                if (!_dicModelName.TryGetValue(unitAttr.UnitName, out var model))
                {
                    log.ErrorFormat("[ModelRepository] Fatal error, unitName:{0} not exist", unitAttr.UnitName);
                    return;
                }
                

                _dicGameModel.Add(modelId, model);
                
                log.DebugFormat("[ModelRepository], modelId:{0}, ModelName:{1}", unitAttr.UnitDataNodeId, unitAttr.UnitName);
                
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
           
        }
        
        public ModelRepository(ICharacterDataRepository characterDataRepository)
        {
            log.Debug("[ModelRepository], Starting Construction");
            _characterDataRepository = characterDataRepository;
            _dicGameModel = new Dictionary<long, UObject>();
            _dicModelName = new Dictionary<string, UObject>();
            
        }
    }
}

