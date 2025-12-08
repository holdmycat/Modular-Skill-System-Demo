//------------------------------------------------------------
// File: IActorInstance.cs
// Purpose: Base actor instance interface exposing numeric data and lifecycle hooks.
//------------------------------------------------------------
using Cysharp.Threading.Tasks;

namespace Ebonor.DataCtrl
{
    
    public interface IActorInstance
    {
        ActorNumericComponentBase ActorNumericComponentBase { get; }

        uint NetId { get; }
        
        /// <summary>Load/prepare actor data and resources.</summary>
        UniTask<bool> LoadAsync<T>(CharacterRuntimeData characterdata) where T : ActorNumericComponentBase;

        /// <summary>Clean up resources (opposite of LoadAsync).</summary>
        UniTask<bool> UnloadAsync();

        /// <summary>Spawn/enter the scene.</summary>
        UniTask<bool> SpawnAsync();

        /// <summary>Despawn/recycle the actor.</summary>
        UniTask<bool> DespawnAsync();
    }
}
