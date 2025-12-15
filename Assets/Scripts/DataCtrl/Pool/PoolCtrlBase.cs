//------------------------------------------------------------
// File: PoolCtrlBase.cs
// Purpose: Base class for pool controllers.
//------------------------------------------------------------
using Ebonor.Framework;
using UnityEngine;
using Zenject;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public abstract class PoolCtrlBase : MonoBehaviour
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(PoolCtrlBase));

        [Inject]
        protected  IInstantiator _instantiator;
        
        /// <summary>Which pool group this controller manages.</summary>
        protected ePoolObjectType poolType;
        /// <summary>Shared pool configuration injected from GlobalGameConfig.</summary>
        protected ResourcePoolConfig resourcePoolConfig;
        
        /// <summary>Per-frame update hook for pool maintenance.</summary>
        public virtual void OnUpdate(PoolManager poolMgr)
        {
            log.DebugFormat("[PoolCtrlBase] OnUpdate poolType={0}", poolType);

            if (resourcePoolConfig == null)
            {
                resourcePoolConfig = GlobalServices.GlobalGameConfig.ResourcePoolConfig;
            }
        }

        /// <summary>Handle pause/resume for pooled items if needed.</summary>
        public abstract void OnPauseResumeGame(PoolManager poolMgr, bool isPause);
        
        /// <summary>Zenject Container for instantiating pooled objects.</summary>
        //protected Zenject.DiContainer _container;

        /// <summary>Initialize pool with its type and container.</summary>
        public virtual void InitPool(ePoolObjectType type, Zenject.DiContainer container)
        {
            log.DebugFormat("[PoolCtrlBase] InitPool type={0}", type);
            poolType = type;
           // _container = container;
        }
        
        /// <summary>Create and register an item in the pool.</summary>
        public virtual void InitPoolItem<T>(string name) where T : PoolItemBase
        {
            log.DebugFormat("[PoolCtrlBase] InitPoolItem type={0} name={1}", typeof(T).Name, name);
        }
        
        /// <summary>Spawn an item by key from the pool.</summary>
        public virtual T SpawnItemFromPool<T>(string _name) where T : PoolItemBase
        {
            log.DebugFormat("[PoolCtrlBase] SpawnItemFromPool type={0} name={1}", typeof(T).Name, _name);
            return null;
        }
        
        /// <summary>Return an item to the pool.</summary>
        public virtual void DespawnItemFromPool<T>(T t) where T : PoolItemBase
        {
            log.DebugFormat("[PoolCtrlBase] DespawnItemFromPool type={0} name={1}", typeof(T).Name, t != null ? t.name : "null");
        }
        
        /// <summary>Clear all pooled items.</summary>
        public abstract void ClearAllPoolItem();
    }
}
