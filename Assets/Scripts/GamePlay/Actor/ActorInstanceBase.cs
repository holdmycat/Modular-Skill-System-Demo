//------------------------------------------------------------
// File: ActorInstanceBase.cs
// Purpose: Base actor instance providing numeric access and lifecycle hooks.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{

    //ActorNumericComponentBase
     public abstract partial class ActorInstanceBase : MonoBehaviour, IActorInstance
     {
         protected ActorNumericComponentBase _actorNumericComponentBase;
         public ActorNumericComponentBase ActorNumericComponentBase => _actorNumericComponentBase;
    
         protected virtual async UniTask<bool> LoadActorNumeric<T>(CharacterRuntimeData characterdata) where T : ActorNumericComponentBase
         {
             if (null == ActorNumericComponentBase)
             {
                 _actorNumericComponentBase = gameObject.AddComponent<T>();
                 _actorNumericComponentBase.OnInitActorNumericComponent(characterdata, _netId);
                 return true;
             }
             
             log.Error("Fatal error, ActorNumericComponentBase not null");
             return false;
         }
         
         protected virtual  async  UniTask<bool> UnloadNumericAsync()
         {
             
             _actorNumericComponentBase.OnUnInitActorNumericComponent();
             _actorNumericComponentBase = null;
             return true;
         }
     }
    
     
     //system
    public abstract partial class ActorInstanceBase : MonoBehaviour, IActorInstance
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ActorInstanceBase));
        
        protected uint _netId;
        public uint NetId => _netId;

        public long GetModelId()
        {
            if (null == _actorNumericComponentBase)
            {
                log.Error("Fatal error, _actorNumericComponentBase is null");
                return -1;
            }

            return _actorNumericComponentBase.UnitModelNodeId;

        }

        /// <summary>
        /// Load/prepare actor data and resources. Override to provide concrete numeric component creation.
        /// </summary>
        public virtual async UniTask<bool> LoadAsync<T>(CharacterRuntimeData characterdata) where T : ActorNumericComponentBase
        {
            // _netId = GlobalServices.NextId();
            //
            // //load numeric data
            // var result = await LoadActorNumeric<T>(characterdata);
            //
            // // In edit-mode tests the pool may be empty; guard against null.
            // var actorModel = PoolManager.SpawnItemFromPool<PoolModelItem>(ePoolObjectType.eModel, _actorNumericComponentBase.AttrAvatarName);
            // if (actorModel != null)
            // {
            //     GOHelper.ResetLocalGameObject(gameObject, actorModel.gameObject, true);
            // }
            // else
            // {
            //     log.Warn($"Actor model '{_actorNumericComponentBase.AttrAvatarName}' not found in pool; skipping placement (likely edit-mode).");
            // }
            //
            // return result;

            return true;
        }

        /// <summary>Cleanup resources.</summary>
        public virtual async UniTask<bool> UnloadAsync()
        {
            var result = await UnloadNumericAsync();
            
            return result;
        }

        /// <summary>Spawn/enter the scene.</summary>
        public virtual async UniTask<bool> SpawnAsync()
        {
            return true;
        }

        /// <summary>Despawn/recycle.</summary>
        public virtual async UniTask<bool> DespawnAsync()
        {
            return true;
        }
    }
}
