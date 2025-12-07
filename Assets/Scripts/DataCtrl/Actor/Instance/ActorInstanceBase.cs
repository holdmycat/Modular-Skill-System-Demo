//------------------------------------------------------------
// File: ActorInstanceBase.cs
// Purpose: Base actor instance providing numeric access and lifecycle hooks.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{

    public abstract class ActorInstanceBase : MonoBehaviour, IActorInstance
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ActorInstanceBase));

        protected ActorNumericComponentBase actorNumericComponent;
        public ActorNumericComponentBase ActorNumericComponentBase => actorNumericComponent;

        /// <summary>
        /// Load/prepare actor data and resources. Override to provide concrete numeric component creation.
        /// </summary>
        public virtual async UniTask<bool> LoadAsync<T>(CharacterRuntimeData characterdata) where T : ActorNumericComponentBase
        {
            var _netId = GlobalServices.NextId();
            
            return true;
        }

        /// <summary>Cleanup resources.</summary>
        public virtual async UniTask<bool> UnloadAsync()
        {
            return true;
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
