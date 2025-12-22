using Cysharp.Threading.Tasks;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerManager));

        private readonly ServerRoomManager _serverRoomManager;
        
        public ServerManager(ServerRoomManager roomManager)
        {
            _serverRoomManager = roomManager;
            log.Debug("[ServerManager] Constructed.");
        }

        public void InitAsync()
        {
            log.Info("[ServerManager] Initializing...");
            _serverRoomManager.InitAsync();
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
