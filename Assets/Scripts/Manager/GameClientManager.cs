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
using UnityEngine;

namespace Ebonor.Manager
{
    /// <summary>
    /// Central runtime controller responsible for global data, scene managers, and lifecycle.
    /// </summary>
    public class GameClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GameClientManager));

        private SceneManagerBase _currentSceneManager;
        private DataCtrl.DataCtrl _dataCtrlInst;

        private void Awake()
        {
            // Keep empty; creation/ownership is handled by scene.
        }
        
        private void OnDestroy()
        {
            // No singleton cleanup required.
        }

        private void Start()
        {
            EnsureDataCtrl();
        }

        private void Update()
        {
            _currentSceneManager?.Tick(Time.deltaTime).Forget();
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

            if (_currentSceneManager != null)
            {
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
