using System;
using Cysharp.Threading.Tasks;
using Zenject;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.UI;

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
        private IProgress<float> progressReporter;
        private UIScene_Loading _uiSceneLoading;
        private readonly IUIService _uiService;
        public GameStartup(IUIService uiService, ISceneLoaderService sceneLoader, IDataLoaderService dataLoaderService, GlobalGameConfig config, ISystemDataService systemDataService)
        {
            _sceneLoader = sceneLoader;
            _dataLoaderService = dataLoaderService;
            _config = config;
            _systemDataService = systemDataService;
            _uiService = uiService;
            progressReporter = new System.Progress<float>(progress =>
            {
                log.Info($"[GameStartup]Global Loading Progress: {progress * 100:F0}%");
                _uiSceneLoading?.SetPercent(progress);
            });     
        }

        public void Initialize()
        {
            // 使用 UniTask.Void 启动异步流程
            StartupSequence().Forget();
        }

        private async UniTask StartupSequence()
        {
            log.Info("[GameStartup] 1. Start System Initialization...");
            
            _uiSceneLoading = await _uiService.OpenUIAsync<UIScene_Loading>();
            
            // 1. 初始化系统数据 (BSON 注册等)
            progressReporter.Report(0.1f);
            await _systemDataService.InitializeAsync();
            
            // 2. 从ResourceLoader中加载资源
            progressReporter.Report(0.3f);
            log.Info("[GameStartup] 2. System Initialized. Loading Data From ResourceLoader...");
            await _dataLoaderService.InitializeAsync(); 
            
            // 3. 数据准备好了，才去加载场景
            // 这里的 LoadSceneAsync 会负责卸载 Bootstrap，加载 Showcase
            log.Info("[GameStartup] 3. Data Loaded. Switching to Game Scene...");
            progressReporter.Report(0.5f);
            await _sceneLoader.LoadSceneAsync(_config.FirstSceneName);
            
            progressReporter.Report(1f);
            await _uiService.CloseUIAsync<UIScene_Loading>();
        }
    }
}
