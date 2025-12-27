using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public  class ShowCaseSceneServerManager: BaseServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowCaseSceneServerManager));

        
        [Inject]
        public ShowCaseSceneServerManager(ServerRoomManager roomManager) : base(roomManager)
        {
            log.Info("[ShowCaseSceneServerManager] Constructed.");
        }
        
        
    }
}
