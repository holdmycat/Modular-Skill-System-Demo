using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.DataCtrls;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    //model
    public abstract partial class GameEntity : MonoBehaviour
    {
        [Inject]
        protected IPoolManager _poolManager;
        
        protected PoolItemBase _currentModel;
        
        /// <summary>
        /// Loads the visual model from the pool and attaches it to this entity.
        /// </summary>
        private  async UniTask LoadModelAsync(string prefabName)
        {
             log.DebugFormat("[GameEntity] LoadModelAsync prefab={0} netId={1}", prefabName, _netId);
             if (_currentModel != null)
             {
                 UnloadModel();
             }

             // Assuming ePoolObjectType.eModel for now. 
             // In a real scenario, this might depend on the entity type.
             _currentModel = _poolManager.SpawnItemFromPool<PoolItemBase>(ePoolObjectType.eModel, prefabName);
             
             if (_currentModel != null)
             {
                 Transform transform1;
                 (transform1 = _currentModel.transform).SetParent(this.transform);
                 transform1.localPosition = Vector3.zero;
                 transform1.localRotation = Quaternion.identity;
                 
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
        private  void UnloadModel()
        {
            log.DebugFormat("[GameEntity] UnloadModel netId={0}", _netId);

            if (_currentModel != null)
            {
                if (_currentModel is IPoolItem poolItem)
                {
                    poolItem.OnDespawn();
                }
                
                // Only return the model to the pool during play mode and while the app is still running.
                if (Application.isPlaying)
                {
                    if (_poolManager == null)
                    {
                        log.Warn("[GameEntity] Skip returning model to pool because pool manager is missing.");
                    }
                    else if (ApplicationQuitHelper.IsQuitting)
                    {
                        log.Debug("[GameEntity] Skip returning model to pool because application is quitting.");
                    }
                    else
                    {
                        _poolManager.DespawnItemToPool(ePoolObjectType.eModel, _currentModel);
                    }
                }
                
                _currentModel = null;
            }
        }
        
    }
    
    //actor numeric component factor
    public abstract partial class GameEntity : MonoBehaviour
    {
        [Inject]
        protected IActorNumericComponentFactory _numericComponentFactory;

        protected ActorNumericComponentBase _numericComponent;
        public ActorNumericComponentBase NumericComponent => _numericComponent;
        
        protected void LoadDataAsync<T>(CharacterRuntimeData runtimeData) where T : ActorNumericComponentBase
        {
            log.DebugFormat("[GameEntity] LoadDataAsync type={0} netId={1}", typeof(T).Name, runtimeData._netId);

            if (_numericComponent == null)
            {
                // Use the Factory to create the component. 
                // The factory handles the dependency injection for the component internally.
                _numericComponent = _numericComponentFactory.Create<T>(gameObject);
                _numericComponent.OnInitActorNumericComponent(runtimeData);
                return;
            }
            
            log.Error("[GameEntity] Fatal error _numericComponent has been created");
        }
    }
    
    /// <summary>
    /// Base class for all game entities in the new architecture.
    /// Acts as the persistent logic shell (Controller).
    /// It manages the visual model (View) which is pooled.
    /// </summary>
    public abstract partial class GameEntity : MonoBehaviour
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(GameEntity));
        
        private uint _netId;
        public uint NetId => _netId;
        
        // Template method: defines the initialization flow, executed once per entity instance.
        public async UniTask FullInitialize(CharacterRuntimeData data, string prefabName)
        {

            _netId = data._netId;
            log.DebugFormat("[GameEntity] Initializing entity {0} with prefab {1}.", _netId, prefabName);
            
            // 1. Load the visual model.
            await LoadModelAsync(prefabName);
        
            // 2. Load the numeric/data component (subclasses decide the concrete type).
            await InitializeDataAsync(data);
            
            log.DebugFormat("[GameEntity] Initialization finished for entity {0}.", _netId);
        }
    
        // Abstract method: subclasses decide which NumericComponent type to use.
        protected abstract UniTask InitializeDataAsync(CharacterRuntimeData data);
        
        protected virtual void OnDestroy()
        {
            log.DebugFormat("[GameEntity] OnDestroy netId={0}", _netId);
            
            if (!Application.isPlaying || ApplicationQuitHelper.IsQuitting) 
            {
                return;
            }
            
            UnloadModel();
            
            if (_numericComponent != null)
            {
                _numericComponent.OnUnInitActorNumericComponent();
            }
        }
    }
}
