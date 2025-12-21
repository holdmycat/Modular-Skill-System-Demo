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
            log.Debug("[ClientPlayer] Constructed.");
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
            return UniTask.CompletedTask;
        }
    }
}
