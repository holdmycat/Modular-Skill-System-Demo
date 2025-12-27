using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public  class ShowCaseSceneClientManager: BaseClientManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowCaseSceneClientManager));

        private readonly ClientRoomManager _serverRoomManager;
        
        
        [Inject]
        public void Construct(ClientRoomManager roomManager)
        {
            log.Info("[ShowCaseSceneClientManager] Constructed.");
            
            _clientRoomManager = roomManager;
            GOHelper.ResetLocalGameObject(gameObject,_clientRoomManager.gameObject,true, 1f);
            log.Info("[ShowCaseSceneClientManager] Constructed (Injected).");
        }

       
    }
}
