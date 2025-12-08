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
        
        private readonly Dictionary<uint, List<IActorInstance>> _pooledActors = new Dictionary<uint, List<IActorInstance>>();
        
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

        public override T SpawnItemFromPool<T>(string name)
        {
            uint id = uint.Parse(name);
            if (!_pooledActors.ContainsKey(id))
            {
                _pooledActors[id] = new List<IActorInstance>();
            }

            var list = _pooledActors[id];

            if (list.Count > 0)
            {
                var result = list[0];
                list.RemoveAt(0);
                return result as T;
            }
            
            MethodInfo methodInfo = typeof(PoolModelCtrl).GetMethod(nameof(GenerateModel), BindingFlags.Instance | BindingFlags.NonPublic)?.MakeGenericMethod(typeof(T));
            if (methodInfo != null)
            {
                var instance = methodInfo.Invoke(this, new object[]{id});
                return instance as T;
            }

            return null;
        }

        public override void DespawnItemFromPool<T>(T t)
        {
            if (t == null) return;
            
            if (t is IActorInstance actorInstance)
            {
                var id = actorInstance.GetModelId();
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
        
        private T GenerateModel<T>(uint heroId) where T : IActorInstance
        {
            UObject modelObj = null;

            if (typeof(T).Name.Equals("DropAnimatorManager"))
            {
                var dic = DataCtrl.Inst.DicDrop;
                dic.TryGetValue(heroId, out modelObj);
            }
            else if (typeof(T).Name.Equals("SummonAnimatorManager"))
            {
                var dic = DataCtrl.Inst.DicSummon;
                if (!dic.TryGetValue(heroId, out modelObj))
                {
                    log.ErrorFormat("GenerateModel: summonId {0} not found.", heroId);
                    return default;
                }
            }
            else if (typeof(T).Name.Equals("BulletAnimatorManager"))
            {
                var dic = DataCtrl.Inst.DicBullet;
                if (!dic.TryGetValue(heroId, out modelObj))
                {
                    log.ErrorFormat("GenerateModel: bulletId {0} not found.", heroId);
                    return default;
                }
            }
            else if (typeof(T).Name.Equals("PlayerAnimatorManager"))
            {
                var heroModels = DataCtrl.Inst.GetGameModelDic(eActorModelType.eHero);
                if (!heroModels.TryGetValue(heroId, out modelObj))
                {
                    var npcModels = DataCtrl.Inst.GetGameModelDic(eActorModelType.eNpc);
                    npcModels.TryGetValue(heroId, out modelObj);
                }
            }
            else
            {
                var npcModels = DataCtrl.Inst.GetGameModelDic(eActorModelType.eNpc);
                npcModels.TryGetValue(heroId, out modelObj);
            }
            
            if (modelObj == null)
            {
                log.ErrorFormat("GenerateModel: model for id {0} not found.", heroId);
                return default;
            }

            var actorGo = (GameObject)GlobalHelper.InstantiatePrefab(modelObj);
            var instance = actorGo.GetComponent<T>();
            if (instance == null)
            {
                log.ErrorFormat("GenerateModel: component {0} missing on model id {1}.", typeof(T).Name, heroId);
                Object.DestroyImmediate(actorGo);
                return default;
            }

            return instance;
        }
    }
}
