using Cysharp.Threading.Tasks;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.Manager
{
    // Pure C# class, no MonoBehaviour needed
    public class GameStartup : IInitializable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(GameStartup));
        
        readonly ISceneLoaderService _sceneLoader;
        readonly GlobalGameConfig _config;
        readonly ISystemDataService _systemDataService;
        readonly IDataLoaderService _dataLoaderService;
        
        public GameStartup(ISceneLoaderService sceneLoader, IDataLoaderService dataLoaderService, GlobalGameConfig config, ISystemDataService systemDataService)
        {
            _sceneLoader = sceneLoader;
            _dataLoaderService = dataLoaderService;
            _config = config;
            _systemDataService = systemDataService;
        }

        public void Initialize()
        {
            // 使用 UniTask.Void 启动异步流程
            StartupSequence().Forget();
        }

        private async UniTask StartupSequence()
        {
            log.Info("[GameStartup] 1. Start System Initialization...");
            
            // 1. 初始化系统数据 (BSON 注册等)
            await _systemDataService.InitializeAsync();
            
            // 2. 从ResourceLoader中加载资源
            log.Info("[GameStartup] 2. System Initialized. Loading Data From ResourceLoader...");
            await _dataLoaderService.InitializeAsync(); 
            
            // 3. 数据准备好了，才去加载场景
            // 这里的 LoadSceneAsync 会负责卸载 Bootstrap，加载 Showcase
            log.Info("[GameStartup] 3. Data Loaded. Switching to Game Scene...");
            await _sceneLoader.LoadSceneAsync(_config.FirstSceneName);
        }
    }
}
