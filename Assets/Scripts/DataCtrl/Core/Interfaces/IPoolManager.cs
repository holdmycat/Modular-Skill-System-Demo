using UnityEngine;

namespace Ebonor.DataCtrl
{
    public interface IPoolManager
    {
        /// <summary>
        /// Spawns an item from the pool.
        /// </summary>
        /// <typeparam name="T">Type of the component to return</typeparam>
        /// <param name="type">Pool type (e.g., Model, Effect)</param>
        /// <param name="name">Name/ID of the item</param>
        /// <returns>The spawned item</returns>
        T SpawnItemFromPool<T>(ePoolObjectType type, string name) where T : PoolItemBase;

        /// <summary>
        /// Despawns an item back to the pool.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <param name="type">Pool type</param>
        /// <param name="item">The item to despawn</param>
        void DespawnItemToPool<T>(ePoolObjectType type, T item) where T : PoolItemBase;

        /// <summary>
        /// Pre-initializes a pool item.
        /// </summary>
        void InitPoolItem<T>(ePoolObjectType type, string name) where T : PoolItemBase;
    }
}
