using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public interface IEntityFactory
    {
        
        UniTask<T> SpawnGameEntity<T>(CharacterRuntimeData characterRuntimeData, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameEntity;
        
        /// <summary>
        /// Despawns an entity (returns to pool or destroys).
        /// </summary>
        /// <param name="entity">The entity to despawn</param>
        void DespawnEntity(PoolItemBase entity);
    }
}
