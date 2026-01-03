using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.GamePlay;
using Zenject;

namespace Ebonor.Manager
{
    public  class ShowCaseSceneServerManager: BaseServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowCaseSceneServerManager));

        
        [Inject]
        public ShowCaseSceneServerManager(ServerRoomManager roomManager, [Inject(Id = ClockIds.Server)] Clock clock) : base(roomManager, clock)
        {
            log.Info("[ShowCaseSceneServerManager] Constructed.");
        }
        
        
    }
}
