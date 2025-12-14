using Ebonor.DataCtrls;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public abstract class PoolItemBase : MonoBehaviour, IPoolItem
    {
        public virtual void OnSpawn() { }
        public virtual void OnDespawn() { }
    }

}
