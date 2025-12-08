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
        protected ePoolObjectType poolType;
        protected ResourcePoolConfig resourcePoolConfig;
        
        public virtual void OnUpdate(PoolManager poolMgr)
        {
            if (resourcePoolConfig == null)
            {
                resourcePoolConfig = GlobalServices.GlobalGameConfig.ResourcePoolConfig;
            }
        }

        public abstract void OnPauseResumeGame(PoolManager poolMgr, bool isPause);
        
        public virtual void InitPool(ePoolObjectType type)
        {
            poolType = type;
        }
        
        public virtual void InitPoolItem<T>(string name) where T : Component { }
        
        public virtual T SpawnItemFromPool<T>(string name) where T : Component { return null; }

        public virtual void DespawnItemFromPool<T>(T t) where T : Component {}
        
        public abstract void ClearAllPoolItem();
    }
}
