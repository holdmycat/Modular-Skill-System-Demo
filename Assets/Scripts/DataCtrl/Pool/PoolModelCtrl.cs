//------------------------------------------------------------
// File: PoolModelCtrl.cs
// Purpose: Pool controller for actor model instances.
//------------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using Ebonor.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public class PoolModelCtrl : PoolCtrlBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PoolModelCtrl));
        
        /// <summary>Pooled actor instances by model id.</summary>
        private readonly Dictionary<string, List<PoolItemBase>> _pooledActors = new Dictionary<string, List<PoolItemBase>>();
        
        public override void ClearAllPoolItem()
        {
            foreach (var list in _pooledActors.Values)
            {
                list.Clear();
            }
            _pooledActors.Clear();
        }

        public override void OnPauseResumeGame(PoolManager poolMgr, bool isPause)
        {
            // No-op for now; implement if pooled actors need pause handling.
        }

        public override T SpawnItemFromPool<T>(string _name)
        {
           
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
            
            // Fallback: generate a new instance.
            MethodInfo methodInfo = typeof(PoolModelCtrl).GetMethod(nameof(GenerateModel), BindingFlags.Instance | BindingFlags.NonPublic)?.MakeGenericMethod(typeof(T));
            if (methodInfo != null)
            {
                var instance = methodInfo.Invoke(this, new object[]{_name});
                return instance as T;
            }

            return null;
        }

        public override void DespawnItemFromPool<T>(T t)
        {
            if (t == null) return;
            
            if (t is PoolItemBase actorInstance)
            {
                var id = t.name;
                GlobalHelper.ResetLocalGameObject(gameObject, t.gameObject, false, 1f);

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
            UObject modelObj = null;

            
            var unitAttributeNodeData = DataCtrl.Inst.GetUnitAttributeNodeDataByUnitName(modelName);
            
            switch (unitAttributeNodeData.ActorModelType)
            {

                case eActorModelType.eHero:
                {
                    var heroModels = DataCtrl.Inst.GetGameModelDic(eActorModelType.eHero);
                    if (!heroModels.TryGetValue(unitAttributeNodeData.UnitDataNodeId, out modelObj))
                    {
                        var npcModels = DataCtrl.Inst.GetGameModelDic(eActorModelType.eNpc);
                        npcModels.TryGetValue(unitAttributeNodeData.UnitDataNodeId, out modelObj);
                    }
                    break;
                }
                
            }
            
            if (modelObj == null)
            {
                log.ErrorFormat("GenerateModel: model for id {0} not found.", unitAttributeNodeData.UnitDataNodeId);
                return default;
            }

            var actorGo = (GameObject)GlobalHelper.InstantiatePrefab(modelObj);
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
