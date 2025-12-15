using System;
using System.Reflection;
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
        private float _progressValue;
        private UIScene_Loading _uiSceneLoading;
        private readonly IUIService _uiService;
        private readonly IVoiceChatService _voiceChatService;
        public GameStartup(IUIService uiService, ISceneLoaderService sceneLoader, IDataLoaderService dataLoaderService, GlobalGameConfig config, ISystemDataService systemDataService, IVoiceChatService voiceChatService)
        {
            _sceneLoader = sceneLoader;
            _dataLoaderService = dataLoaderService;
            _config = config;
            _systemDataService = systemDataService;
            _uiService = uiService;
            _voiceChatService = voiceChatService;
            progressReporter = new System.Progress<float>(progress =>
            {
                log.Info($"[GameStartup]Global Loading Progress: {progress * 100:F0}%");
                _uiSceneLoading?.SetPercent(progress);
                _progressValue = progress;
            });     
        }

        public void Initialize()
        {
#if UNITY_EDITOR
            if (IsRunningPlaymodeTests())
            {
                log.Info("[GameStartup] Detected playmode test runner; skipping startup sequence.");
                return;
            }
#endif
            // Kick off async flow without awaiting
            StartupSequence().Forget();
        }

        private async UniTask StartupSequence()
        {
            log.Info("[GameStartup] 1. Start System Initialization...");
            
            _uiSceneLoading = await _uiService.OpenUIAsync<UIScene_Loading>();
            
            // 1. Initialize system data (BSON registration, etc.)
            progressReporter.Report(0.1f);
            await RunCriticalModule("SystemData", () => _systemDataService.InitializeAsync());
            
            // 2. Load data from ResourceLoader
            progressReporter.Report(0.3f);
            log.Info("[GameStartup] 2. System Initialized. Loading Data From ResourceLoader...");
            await RunCriticalModule("DataLoader", () => _dataLoaderService.InitializeAsync()); 

            // 2.5 Optional modules (graceful degradation)
            await RunOptionalModule("VoiceChat", () => _voiceChatService.InitializeAsync(), () =>
            {
                _uiSceneLoading?.SetTitle("语音暂不可用，稍后再试");
            });
            
            // 3. After data is ready, switch scene (LoadSceneAsync handles unloading bootstrap and loading Showcase)
            log.Info("[GameStartup] 3. Data Loaded. Switching to Game Scene...");
            progressReporter.Report(0.5f);
            await RunCriticalModule("SceneLoader", () => _sceneLoader.LoadSceneAsync(_config.FirstSceneName));
            
            progressReporter.Report(1f);
            await UniTask.WaitUntil(() => _progressValue >= 0.999f);
            await UniTask.Delay(250);
            await _uiService.CloseUIAsync<UIScene_Loading>();
        }

        /// <summary>
        /// Helper to run optional modules without blocking startup. Logs and executes a fallback if they fail.
        /// </summary>
        private async UniTask RunOptionalModule(string name, Func<UniTask> action, System.Action onFailure = null)
        {
            try
            {
                await action();
                log.Info($"[GameStartup] Optional module {name} initialized.");
            }
            catch (System.Exception ex)
            {
                log.Error($"[GameStartup] Optional module {name} failed; continuing startup. {ex}");
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Critical path step: log and rethrow to fail fast while keeping consistent surface for UI/logs.
        /// </summary>
        private async UniTask RunCriticalModule(string name, Func<UniTask> action)
        {
            try
            {
                await action();
                log.Info($"[GameStartup] Critical module {name} initialized.");
            }
            catch (System.Exception ex)
            {
                log.Error($"[GameStartup] Critical module {name} failed; aborting startup. {ex}");
                _uiSceneLoading?.SetTitle($"{name} 初始化失败，请重试");
                throw;
            }
        }

#if UNITY_EDITOR
        private static bool IsRunningPlaymodeTests()
        {
            // Unity test framework attaches a PlaymodeTestsController named "Code-based tests runner"
            var type = Type.GetType("UnityEngine.TestTools.TestRunner.PlaymodeTestsController, UnityEngine.TestRunner");
            var method = type?.GetMethod("IsControllerOnScene", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                return false;
            }

            return method.Invoke(null, null) is bool running && running;
        }
#endif
    }
}
