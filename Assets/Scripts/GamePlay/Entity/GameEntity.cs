using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.DataCtrls;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Base class for all game entities in the new architecture.
    /// Acts as the persistent logic shell (Controller).
    /// It manages the visual model (View) which is pooled.
    /// </summary>
    public abstract class GameEntity : MonoBehaviour
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(GameEntity));

        [Inject]
        protected IPoolManager _poolManager;

        [Inject]
        protected DiContainer _container;

        [SerializeField] private uint _netId;
        public uint NetId => _netId;

        protected PoolItemBase _currentModel;

        /// <summary>
        /// Async initialization method.
        /// </summary>
        public virtual async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Loads the visual model from the pool and attaches it to this entity.
        /// </summary>
        public virtual async UniTask LoadModelAsync(string prefabName)
        {
             if (_currentModel != null)
             {
                 UnloadModel();
             }

             // Assuming ePoolObjectType.eModel for now. 
             // In a real scenario, this might depend on the entity type.
             _currentModel = _poolManager.SpawnItemFromPool<PoolItemBase>(ePoolObjectType.eModel, prefabName);
             
             if (_currentModel != null)
             {
                 _currentModel.transform.SetParent(this.transform);
                 _currentModel.transform.localPosition = Vector3.zero;
                 _currentModel.transform.localRotation = Quaternion.identity;
                 
                 // If the model implements IPoolItem, OnSpawn is handled by the PoolManager (if updated) 
                 // or we can manually call it here if PoolManager doesn't support it yet.
                 // For safety in this transition, let's check and call it if needed, 
                 // but ideally PoolManager does it.
                 if (_currentModel is IPoolItem poolItem)
                 {
                     poolItem.OnSpawn();
                 }
             }
             else
             {
                 log.Error($"[GameEntity] Failed to load model: {prefabName}");
             }
             
             await UniTask.CompletedTask;
        }

        /// <summary>
        /// Unloads the visual model, returning it to the pool.
        /// </summary>
        public virtual void UnloadModel()
        {
            if (_currentModel != null)
            {
                if (_currentModel is IPoolItem poolItem)
                {
                    poolItem.OnDespawn();
                }
                
                _poolManager.DespawnItemToPool(ePoolObjectType.eModel, _currentModel);
                _currentModel = null;
            }
        }
        
        protected virtual void OnDestroy()
        {
            UnloadModel();
        }
    }
}
