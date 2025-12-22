//------------------------------------------------------------
// File: PoolModelCtrl.cs
// Purpose: Pool controller for actor model instances.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using Ebonor.Framework;
using UnityEngine;
using Zenject;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public class PoolModelCtrl : PoolCtrlBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PoolModelCtrl));
        
        /// <summary>Pooled actor instances by model id.</summary>
        private readonly Dictionary<string, List<PoolItemBase>> _pooledActors = new Dictionary<string, List<PoolItemBase>>();

        [Inject]
        private ICharacterDataRepository _characterDataRepository;
        
        [Inject]
        private IModelRepository _modelRepository;
        
        public override void ClearAllPoolItem()
        {
            log.Debug("[PoolModelCtrl] ClearAllPoolItem");

            foreach (var list in _pooledActors.Values)
            {
                list.Clear();
            }
            _pooledActors.Clear();
        }

        public override void OnPauseResumeGame(PoolManager poolMgr, bool isPause)
        {
            log.DebugFormat("[PoolModelCtrl] OnPauseResumeGame isPause={0}", isPause);
            // No-op for now; implement if pooled actors need pause handling.
        }

        public override T SpawnItemFromPool<T>(string _name)
        {
            log.DebugFormat("[PoolModelCtrl] SpawnItemFromPool name={0}", _name);
           
            if (!_pooledActors.ContainsKey(_name))
            {
                _pooledActors[_name] = new List<PoolItemBase>();
            }

            var list = _pooledActors[_name];

            if (list.Count > 0)
            {
                var result = list[0];
                list.RemoveAt(0);
                return result as T;
            }

            var instance = GenerateModel<T>(_name);
            
            return instance;
        }

        public override void DespawnItemFromPool<T>(T t)
        {
            log.DebugFormat("[PoolModelCtrl] DespawnItemFromPool name={0}", t != null ? t.name : "null");

            if (t == null) return;
            
            if (t is PoolItemBase actorInstance)
            {
                var id = t.name;
                GOHelper.ResetLocalGameObject(gameObject, t.gameObject, false, 1f);

                if (_pooledActors.ContainsKey(id))
                {
                    _pooledActors[id].Add(actorInstance);
                }
                else
                {
                    log.ErrorFormat("Actor model id:{0} not found in pool dictionary.", id);
                }
            }
        }
        
        /// <summary>
        /// Instantiate a model prefab for the requested unit name and return the component of type T.
        /// </summary>
        private T GenerateModel<T>(string modelName) where T : PoolItemBase
        {
            
            var unitAttributeNodeData = _characterDataRepository.GetSlgUnitAttributeNodeDataByUnitName(modelName);

            UObject modelObj = _modelRepository.GetGameModel(unitAttributeNodeData.UnitDataNodeId);
            
            if (modelObj == null)
            {
                log.ErrorFormat("GenerateModel: model for id {0} not found.", unitAttributeNodeData.UnitDataNodeId);
                return default;
            }

            // Use Zenject to instantiate so that [Inject] works on the new object.
            // InstantiatePrefab returns a GameObject.
            var actorGo = _instantiator.InstantiatePrefab(modelObj);
            actorGo.name = modelObj.name;
            // Ensure it's active and reset transform if needed (InstantiatePrefab usually keeps prefab settings)
            // But we might want to ensure it's clean.
            GOHelper.ResetGameObject(actorGo);

            var instance = actorGo.GetComponent<T>();
            if (instance == null)
            {
                //log.ErrorFormat("GenerateModel: component {0} missing on model id {1}.", typeof(T).Name, heroId);
                Object.DestroyImmediate(actorGo);
                return default;
            }

            return instance;
        }
    }
}
