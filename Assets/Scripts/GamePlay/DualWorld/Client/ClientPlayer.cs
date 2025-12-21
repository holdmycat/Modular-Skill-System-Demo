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
            BindId(netId);//client player
            // _networkBus.RegisterRpcListener(_netId, OnRpcReceived); // Removed: Handled by Local Object logic if needed, but Spawning is done by Manager now.
             
            log.Debug("[ClientPlayer] Constructed.");
        }

        private void OnRpcReceived(IRpc rpc)
        {
            // Legacy Logic Removed
            log.Info($"<color=magenta>[ClientPlayer] OnRpcReceived [Net:Rpc] {this.GetType().Name}</color>");
        }
        
        
        public override async UniTask InitAsync()
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
