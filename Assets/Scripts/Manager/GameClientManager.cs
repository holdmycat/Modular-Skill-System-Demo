//------------------------------------------------------------
// File: GameClientManager.cs
// Created: 2025-12-06
// Purpose: Central runtime controller for global data, scene managers, and lifecycle.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.UI;
using UnityEngine;

namespace Ebonor.Manager
{
    //ui manager
    public partial class GameClientManager : MonoBehaviour
    {
        private UIManager _uiManager;
        
        private void InitUIManagerService()
        {
            if (null == _uiManager)
            {
                // 2. UI Manager
                var uiGo = new GameObject("UIManager");
                uiGo.transform.SetParent(transform);
                _uiManager = uiGo.AddComponent<UIManager>();
                _uiManager.Init(transform);
                _uiManager.SetGlobalUiHandler(HandleGlobalUiCommand);
                log.Info("UIManager initialized.");
            }
            else
            {
                log.Error("Fatal error, _uiManager should be null");
            }
           
        }

        private void UnInitUIManagerService()
        {
            // Cleanup services
            if (_uiManager != null) _uiManager.Exit();
        }

        /// <summary>Expose UIManager to scene managers for UI orchestration.</summary>
        public UIManager GetUiManager() => _uiManager;

        /// <summary>
        /// Global UI key handling (e.g., back/menu) forwarded by UIManager.
        /// </summary>
        private void HandleGlobalUiCommand(UIManager.UiGlobalCommand command)
        {
            switch (command)
            {
                case UIManager.UiGlobalCommand.Cancel:
                    // TODO: handle global back (e.g., close top UI or exit scene)
                    log.Info("Global UI Cancel triggered.");
                    break;
                case UIManager.UiGlobalCommand.OpenMenu:
                    // TODO: open/pause main menu or forward to scene manager
                    log.Info("Global UI OpenMenu triggered.");
                    break;
            }
        }
    }
    
    //player input router
    public partial class GameClientManager : MonoBehaviour
    {
        private PlayerInputRouter _inputRouter;
        private GameObject _inputSourceInstance;
        private MonoBehaviour _inputSourceBehaviour;
        
        private async UniTask InitPlayerRouterService()
        {
            if (null == _inputRouter)
            {
                // 1. Input Router
                var inputGo = new GameObject("GlobalInputRouter");
                inputGo.transform.SetParent(transform);
                _inputRouter = inputGo.AddComponent<PlayerInputRouter>();

                var source = await CreateInputSource();
                _inputRouter.InitPlayerInputRouter(source);
                GlobalServices.SetPlayerInputSource(_inputRouter);
                log.Info("GlobalInputRouter initialized.");
            }
            else
            {
                log.Error("Fatal error, _inputRouter should be null");
            }
        }

        private async UniTask<IPlayerInputSource> CreateInputSource()
        {
            // Prefer Unity Input System prefab if present; otherwise fall back to legacy keyboard input.
            string prefabPath = GlobalServices.GlobalGameConfig != null
                ? GlobalServices.GlobalGameConfig.playerInputPrefabPath
                : ConstData.UI_PLAYERACTION;

            if (GlobalServices.ResourceLoader == null)
            {
                log.Error("Fatal error, GlobalServices.ResourceLoader is null");
                return null;
            }

            var prefab = await GlobalServices.ResourceLoader.LoadAsset<GameObject>(prefabPath, ResourceAssetType.UiPrefab);
            
            if (prefab != null)
            {
                _inputSourceInstance = Instantiate(prefab, transform);
                _inputSourceInstance.name = prefab.name;

                var inputSource = _inputSourceInstance.GetComponent<InputSystemPlayerInputSource>();
                if (inputSource == null)
                {
                    inputSource = _inputSourceInstance.AddComponent<InputSystemPlayerInputSource>();
                }

                _inputSourceBehaviour = inputSource;
                return inputSource;
            }

            var keyboardSource = gameObject.AddComponent<KeyboardPlayerInputSource>();
            _inputSourceBehaviour = keyboardSource;
            return keyboardSource;
        }

        private void UnInitPlayerRouterService()
        {
            if (_inputRouter != null) 
                _inputRouter.Exit();
            GlobalServices.SetPlayerInputSource(null);

            if (_inputSourceInstance != null)
            {
                Destroy(_inputSourceInstance);
                _inputSourceInstance = null;
            }
            else if (_inputSourceBehaviour != null)
            {
                Destroy(_inputSourceBehaviour);
            }

            _inputSourceBehaviour = null;
            _inputRouter = null;
        }
    }
    
    /// <summary>
    /// Central runtime controller responsible for global data, scene managers, and lifecycle.
    /// </summary>
    public partial class GameClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GameClientManager));
        
        private static GameClientManager instance;
        
        public static GameClientManager Instance => instance;
        
        private SceneManagerBase _currentSceneManager;
        private DataCtrl.DataCtrl _dataCtrlInst;
        
        private void Awake()
        {
            // Keep empty; creation/ownership is handled by scene.
            instance = this;
        }
        
        private void OnDestroy()
        {
            UnInitPlayerRouterService();
            UnInitUIManagerService();
            instance = null;
        }
        
        private void Update()
        {
            float dt = Time.deltaTime;
            
            // Update Services
            if (_inputRouter != null) _inputRouter.OnUpdate(dt);
            if (_uiManager != null) _uiManager.OnUpdate(dt, _inputRouter);

            if (null != _currentSceneManager)
            {
                _currentSceneManager.Tick(dt);
                var poolManager = PoolManager.Inst;
                if (poolManager != null && !poolManager.Equals(null))
                {
                    poolManager.OnUpdate();
                }
            }
        }
        
        public async UniTask InitGameClientManager()
        {
            // Ensure DataCtrl and default setup are ready.
            // Reason: We need BSON registration + data controller in place before any services boot.
            EnsureDataCtrl();

            await InitPlayerRouterService();

            InitUIManagerService();
        }
        
        private async void OnApplicationQuit()
        {
            log.Info("Application quitting. Exiting current scene manager.");
            if (_currentSceneManager != null)
            {
                await _currentSceneManager.Exit();
            }
        }
        
        private async void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                log.Info("Application paused. Notifying scene manager.");
                if (_currentSceneManager != null)
                    await _currentSceneManager.Pause(true);
            }
            else
            {
                log.Info("Application resumed. Notifying scene manager.");
                if (_currentSceneManager != null)
                    await _currentSceneManager.Pause(false);
            }
        }
        
        /// <summary>
        /// Ensure DataCtrl component exists and performs BSON registration.
        /// </summary>
        public void EnsureDataCtrl()
        {
            if (_dataCtrlInst != null) return;

            var go = new GameObject(nameof(DataCtrl));
            _dataCtrlInst = go.AddComponent<DataCtrl.DataCtrl>();
            _dataCtrlInst.transform.SetParent(transform);
            _dataCtrlInst.InitializeDataCtrl();
            
            DataCtrlHelper.OnInitDataCtrlHelper();
            
            
            
            log.Info("DataCtrl initialized.");
        }

        private static PoolManager GetOrCreatePoolManager()
        {
            if (PoolManager.Inst == null || PoolManager.Inst.Equals(null))
            {
                PoolManager.CreatePoolManager();
            }

            return PoolManager.Inst;
        }
        
        /// <summary>
        /// Switch to a new scene manager instance. Destroys the previous manager.
        /// </summary>
        private async UniTask<SceneManagerBase> SwitchSceneManager(SceneManagerBase newSceneManagerInstance)
        {
            if (newSceneManagerInstance == null)
            {
                log.Error("SwitchSceneManager failed: new scene manager is null.");
                return null;
            }

#if UNITY_EDITOR
            // Ensure core data controller exists before any loading.
            EnsureDataCtrl();
#endif

            //load necessary resources
            if (!GlobalServices.IsAppInitialized)
            {
                log.Info("First time initialization: Loading global resources...");

                UIScene_Loading uiLoading = null;
                IProgress<float> progressReporter;
                if (_uiManager != null)
                {
                    uiLoading = await _uiManager.OpenUIAsync<UIScene_Loading>();
                    progressReporter = new System.Progress<float>(progress =>
                    {
                        log.Info($"Global Loading Progress: {progress * 100:F0}%");
                        uiLoading?.SetPercent(progress);
                    });
                }
                else
                {
                    // Fallback for edit-mode tests that don't bootstrap UI.
                    progressReporter = new System.Progress<float>(_ => { });
                }

                GetOrCreatePoolManager();
                
                // Execute the loading pipeline
                uiLoading?.SetTitle("Loading DLL");
                await _dataCtrlInst.LoadAllSystemDataAsync(progressReporter);
                
                //Execute the game data
                uiLoading?.SetTitle("Loading Game Data");
                await _dataCtrlInst.LoadAllGameDataAsync(progressReporter);

                if (null != uiLoading)
                {
                    await _uiManager.CloseUIAsync<UIScene_Loading>(true);
                }

                // Mark as initialized to prevent future re-loading
                GlobalServices.MarkAppInitialized();
            }
            
            
            if (_currentSceneManager != null)
            {
                var poolManager = GetOrCreatePoolManager();
                poolManager?.DoBeforeLeavingScene();
                DataEventManager.OnClearAllDicDELEvents();
                var previous = _currentSceneManager;
                await previous.Exit();
                // In edit mode, Destroy throws; use DestroyImmediate to keep tests safe.
                if (Application.isPlaying)
                    Destroy(previous.gameObject);
                else
                    DestroyImmediate(previous.gameObject);
            }

            _currentSceneManager = newSceneManagerInstance;
            await _currentSceneManager.Init(this);
            var activePoolManager = GetOrCreatePoolManager();
            activePoolManager?.DoBeforeEnteringScene(gameObject.scene.name);
            await _currentSceneManager.Enter();
            log.Info($"Switched to scene manager: {_currentSceneManager.GetType().Name}");
            return _currentSceneManager;
        }

        /// <summary>
        /// Instantiate a scene manager (from prefab if provided) and switch to it.
        /// </summary>
        public async UniTask<T> SwitchSceneManager<T>(T prefab = null) where T : SceneManagerBase
        {
            if (null == prefab)
            {
                log.Error("Fatal error, prefab is null");
                return null;
            }

            // Reason: Tests/CI may call SwitchSceneManager directly; ensure DataCtrl is present for data loading.
            EnsureDataCtrl();
            SceneManagerBase instance = Instantiate(prefab, transform);
            
            instance.name = prefab.name;
            
            instance.transform.SetParent(transform);
                
            return await SwitchSceneManager(instance) as T;
        }

        /// <summary>
        /// Reset the active scene manager (if any).
        /// </summary>
        public UniTask ResetActiveScene()
        {
            return _currentSceneManager != null ? _currentSceneManager.ResetScene() : UniTask.CompletedTask;
        }
    }
}
