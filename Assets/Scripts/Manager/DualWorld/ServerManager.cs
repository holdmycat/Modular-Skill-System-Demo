using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.Manager
{
    public class ServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerManager));

        private readonly ServerRoomManager _serverRoomManager;
        
        // In the future, we might inject ServerTickManager here
        // private readonly ServerTickManager _tickManager;

        public ServerManager(ServerRoomManager roomManager)
        {
            _serverRoomManager = roomManager;
            log.Debug("[ServerManager] Constructed.");
        }

        public async UniTask InitAsync()
        {
            log.Info("[ServerManager] Initializing...");
            await _serverRoomManager.InitAsync();
        }

        public void Tick(int tick)
        {
            // Drive logical updates
            _serverRoomManager.Tick(tick);
        }

        public async UniTask ShutdownAsync()
        {
            log.Info("[ServerManager] Shutting down...");
            await _serverRoomManager.ShutdownAsync();
        }
    }
}
