//------------------------------------------------------------
// File: PoolCtrlBase.cs
// Purpose: Base class for pool controllers.
//------------------------------------------------------------
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Ebonor.DataCtrl
{
    public abstract class PoolCtrlBase : MonoBehaviour
    {
        /// <summary>Which pool group this controller manages.</summary>
        protected ePoolObjectType poolType;
        /// <summary>Shared pool configuration injected from GlobalGameConfig.</summary>
        protected ResourcePoolConfig resourcePoolConfig;
        
        /// <summary>Per-frame update hook for pool maintenance.</summary>
        public virtual void OnUpdate(PoolManager poolMgr)
        {
            if (resourcePoolConfig == null)
            {
                resourcePoolConfig = GlobalServices.GlobalGameConfig.ResourcePoolConfig;
            }
        }

        /// <summary>Handle pause/resume for pooled items if needed.</summary>
        public abstract void OnPauseResumeGame(PoolManager poolMgr, bool isPause);
        
        /// <summary>Initialize pool with its type.</summary>
        public virtual void InitPool(ePoolObjectType type)
        {
            poolType = type;
        }
        
        /// <summary>Create and register an item in the pool.</summary>
        public virtual void InitPoolItem<T>(string name) where T : Component { }
        
        /// <summary>Spawn an item by key from the pool.</summary>
        public virtual T SpawnItemFromPool<T>(string name) where T : Component { return null; }

        /// <summary>Return an item to the pool.</summary>
        public virtual void DespawnItemFromPool<T>(T t) where T : Component {}
        
        /// <summary>Clear all pooled items.</summary>
        public abstract void ClearAllPoolItem();
    }
}
