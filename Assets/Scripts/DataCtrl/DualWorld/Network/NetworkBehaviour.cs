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

        protected ISceneResourceManager _sceneResourceManager;

        
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

        public virtual void InitAsync()
        {
            log.Info($"[NetworkBehaviour] InitAsync");
        }

        

        public virtual async UniTask InitAsync(RpcSpawnObject msg)
        {
            log.Info($"[NetworkBehaviour] InitAsync");
        }
        
        
        public virtual async UniTask ShutdownAsync()
        {
            log.Info($"[NetworkBehaviour] ShutdownAsync");
            if (_netid == 0)
            {
                throw new System.InvalidOperationException($"{GetType().Name} ShutdownAsync called with NetId==0 (not registered).");
            }
        }
        
        public virtual void OnUpdate()
        {
            
        }
        
        public virtual void OnRpc(IRpc rpc)
        {
            log.Warn($"[NetworkBehaviour] Unhandled RPC: {rpc?.GetType().Name} on NetId:{NetId}");
        }

        /// <summary>
        /// Safely spawns a child network object by ensuring the RPC is sent BEFORE server-side initialization.
        /// This prevents race conditions where the child tries to spawn sub-children before the client knows about the parent.
        /// </summary>
        protected void SpawnChild(INetworkBus networkBus, INetworkBehaviour child, byte[] payloadBytes, NetworkPrefabType type, bool isServer)
        {
            // 1. Send RPC (Critical: Must be first)
            networkBus.SendRpc(_netId, new RpcSpawnObject
            {
                Type = type,
                NetId = child.NetId,
                Payload = payloadBytes
            });

            // 2. Initialize Server Side
            if (isServer)
            {
                child.InitAsync();
            }
        }
    }
}
