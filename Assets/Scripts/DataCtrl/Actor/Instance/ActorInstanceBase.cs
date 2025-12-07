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
        public virtual UniTask LoadAsync()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Cleanup resources.</summary>
        public virtual UniTask UnloadAsync()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Spawn/enter the scene.</summary>
        public virtual UniTask SpawnAsync()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Despawn/recycle.</summary>
        public virtual UniTask DespawnAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}
