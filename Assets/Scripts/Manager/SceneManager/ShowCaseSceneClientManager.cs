using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.GamePlay;
using Ebonor.UI;
using Zenject;

namespace Ebonor.Manager
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
        
        // Injected Services
        private ShowcaseContext _showcaseContext;
        private SignalBus _signalBus;
        private IInstantiator _container;

        [Inject]
        public void Construct(
            ClientRoomManager roomManager, 
            GlobalGameConfig globalGameConfig, 
            IScenarioIdRegistry scenarioIdRegistry, 
            UIManager uiManager,
            ShowcaseContext showcaseContext,
            SignalBus signalBus,
            IInstantiator container)
        {
            log.Info("[ShowCaseSceneClientManager] Constructed.");
            
            _clientRoomManager = roomManager;
            _globalGameConfig = globalGameConfig;
            _gameSceneConfig = _globalGameConfig.GameSceneConfig;
            _scenarioIdRegistry = scenarioIdRegistry;
            _uiManager = uiManager;
            _showcaseContext = showcaseContext;
            _signalBus = signalBus;
            _container = container;
            
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
                // UI Init Logic
                log.Info($"[ShowCaseSceneClientManager] UI Opened. Context Available: {_showcaseContext != null}");
            }, _container);
            
            log.Info("[ShowCaseSceneClientManager] OnInitAsync, sceneName:" + sceneName);
        }

        public override async UniTask ShutdownAsync()
        {
            
            log.Info("[ShowCaseSceneClientManager] ShutdownAsync");
            
            if (null != _uiSceneShowCaseScene)
            {
                await _uiManager.CloseUIAsync(_uiSceneShowCaseScene, true);
            }
            
            await base.ShutdownAsync();
        }
    }
}
