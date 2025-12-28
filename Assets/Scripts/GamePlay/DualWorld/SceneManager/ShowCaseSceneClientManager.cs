using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.UI;
using Zenject;

namespace Ebonor.GamePlay
{
    public  class ShowCaseSceneClientManager: BaseClientManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowCaseSceneClientManager));

        private readonly ClientRoomManager _serverRoomManager;
        private GlobalGameConfig _globalGameConfig;
        private GameSceneConfig _gameSceneConfig;
        private IScenarioIdRegistry _scenarioIdRegistry;
        private UIScene_ShowCaseScene _uiSceneShowCaseScene;
        private UIManager _uiManager;
        [Inject]
        public void Construct(ClientRoomManager roomManager, GlobalGameConfig globalGameConfig, IScenarioIdRegistry scenarioIdRegistry, UIManager uiManager)
        {
            log.Info("[ShowCaseSceneClientManager] Constructed.");
            
            _clientRoomManager = roomManager;
            _globalGameConfig = globalGameConfig;
            _gameSceneConfig = _globalGameConfig.GameSceneConfig;
            _scenarioIdRegistry = scenarioIdRegistry;
            _uiManager = uiManager;
            GOHelper.ResetLocalGameObject(gameObject,_clientRoomManager.gameObject,true, 1f);
            log.Info("[ShowCaseSceneClientManager] Constructed (Injected).");
        }


        protected override async UniTask OnInitAsync()
        {
            var sceneName = gameObject.scene.name;

            var sceneId = _scenarioIdRegistry.Normalize(sceneName);

            GameSceneResource gameSceneResource = null;
            
            foreach (var variable in _gameSceneConfig.ListSceneRes)
            {
                if (variable.sceneId.Equals(sceneId))
                {
                    gameSceneResource = variable;
                    break;
                }
            }
            
            if (null == gameSceneResource)
            {
                log.ErrorFormat("[ShowCaseSceneClientManager] OnInitAsync, sceneId{0} not found at _gameSceneConfig.ListSceneRes" + sceneId);
                return;
            }

            _uiSceneShowCaseScene = await _uiManager.OpenUIAsync<UIScene_ShowCaseScene>((uiSceneShowCaseScene) =>
            {
                
            });
            
            
            
            log.Info("[ShowCaseSceneClientManager] OnInitAsync, sceneName:" + sceneName);
        }
       
    }
}
