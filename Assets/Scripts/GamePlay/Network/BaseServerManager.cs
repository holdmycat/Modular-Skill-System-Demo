using Cysharp.Threading.Tasks;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public abstract class BaseServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseServerManager));

        protected readonly ServerRoomManager _serverRoomManager;
        
        public BaseServerManager(ServerRoomManager roomManager)
        {
            _serverRoomManager = roomManager;
            log.Info("[BaseServerManager] Constructed.");
        }

        public virtual async UniTask InitAsync()
        {
            log.Info("[BaseServerManager] Initializing...");
            _serverRoomManager.InitAsync();
        }

        public virtual void Tick(int tick)
        {
            // Drive logical updates
            _serverRoomManager.Tick(tick);
        }

        public virtual async UniTask ShutdownAsync()
        {
            log.Info("[BaseServerManager] Shutting down...");
            await _serverRoomManager.ShutdownAsync();
        }
    }
}
