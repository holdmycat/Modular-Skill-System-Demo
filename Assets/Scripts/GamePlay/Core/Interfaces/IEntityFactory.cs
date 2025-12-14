using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public interface IEntityFactory
    {
        /// <summary>
        /// Spawns an entity asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity (must inherit from ActorInstanceBase)</typeparam>
        /// <param name="assetId">The resource path or ID of the prefab</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <param name="parent">Optional parent transform</param>
        /// <returns>The spawned entity instance</returns>
        UniTask<T> SpawnEntityAsync<T>(long assetId, Vector3 position, Quaternion rotation, Transform parent = null) where T : PoolItemBase;

        /// <summary>
        /// Despawns an entity (returns to pool or destroys).
        /// </summary>
        /// <param name="entity">The entity to despawn</param>
        void DespawnEntity(PoolItemBase entity);
    }
}
