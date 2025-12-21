using Cysharp.Threading.Tasks;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public abstract class NetworkBehaviour : INetworkBehaviour
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkBehaviour));
        
        private uint _netid = 0;
        protected uint _netId => _netid;
        public uint NetId => _netid;

        public void BindId(uint netid)
        {
            if (_netid != 0)
            {
                log.Error("[NetworkBehaviour] Fatal error, _netId is " + _netid);
                return;
            }
            _netid = netid;
        }
        
        public virtual void Tick(int tick)
        {
        }

        public virtual async UniTask InitAsync()
        {
            
        }

        public virtual async UniTask ShutdownAsync()
        {
            
        }
    }
}
