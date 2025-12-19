using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Wraps server-side player state so we can hang all player data and logic off a single object.
    /// </summary>
    internal sealed class ClientPlayer : BaseActor
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientPlayer));
        
        public ClientPlayer( INetworkBus networkBus, uint netId)
        {
            _networkBus = networkBus;
            _netId = netId;
            _networkBus.RegisterRpcListener(_netId, OnRpcReceived);
            log.Debug("[ClientPlayer] Constructed.");
        }

        private void OnRpcReceived(IRpc rpc)
        {
            log.Info($"<color=magenta>[ClientPlayer] OnRpcCreateTeam [Net:Rpc] {this.GetType().Name}</color>");

            if (rpc is RpcCreateTeam rpcCreateTeam)
            {
                
            }
            
        }
        
        
        public override void InitializeTeam()
        {
            log.Info("[ClientPlayer] InitializeTeam");
        }
        

        public override void Tick(int tick)
        {
            
        }

        public override UniTask ShutdownAsync()
        {
            // Placeholder for per-player cleanup/hooks.
            log.Debug("[ClientPlayer] ShutdownAsync.");
            if (_networkBus != null)
                _networkBus.UnregisterRpcListener(_netId, OnRpcReceived);
            return UniTask.CompletedTask;
        }
    }
}
