using Ebonor.DataCtrls;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    
    public abstract class PoolItemBase : MonoBehaviour, IPoolItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PoolItemBase));

        public virtual void OnSpawn()
        {
            log.DebugFormat("[PoolItemBase] OnSpawn {0}", name);
        }

        public virtual void OnDespawn()
        {
            log.DebugFormat("[PoolItemBase] OnDespawn {0}", name);
        }
    }

}
