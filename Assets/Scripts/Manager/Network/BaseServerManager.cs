using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.GamePlay;

namespace Ebonor.Manager
{
    public abstract class BaseServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseServerManager));

        protected readonly ServerRoomManager _serverRoomManager;
        protected readonly Clock _clock;
        
        public BaseServerManager(ServerRoomManager roomManager, Clock clock)
        {
            _serverRoomManager = roomManager;
            _clock = clock;
            log.Info("[BaseServerManager] Constructed.");
        }

        public virtual async UniTask InitAsync()
        {
            log.Info("[BaseServerManager] Initializing...");
            _serverRoomManager.InitAsync();
        }

        //server event frame
        public virtual void Tick(int tick)
        {
            _serverRoomManager.Tick(tick);
        }
        
        //server physics frame
        public void OnFixedUpdate(float deltaTime)
        {
            _clock?.OnFixedUpdate(deltaTime);
        }

        public virtual async UniTask ShutdownAsync()
        {
            log.Info("[BaseServerManager] Shutting down...");
            await _serverRoomManager.ShutdownAsync();
        }
    }
}
