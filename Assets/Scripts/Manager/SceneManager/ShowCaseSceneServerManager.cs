using Ebonor.Framework;
using Ebonor.GamePlay;
using Zenject;

namespace Ebonor.Manager
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
