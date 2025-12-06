//------------------------------------------------------------
// File: GameClientManager.cs
// Created: 2025-12-06
// Purpose: Central runtime controller for global data, scene managers, and lifecycle.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
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

        private void Start()
        {
            EnsureDataCtrl();
        }

        private void Update()
        {
            _currentSceneManager?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            // No singleton cleanup required.
        }

        private void OnApplicationQuit()
        {
            log.Info("Application quitting. Exiting current scene manager.");
            _currentSceneManager?.Exit();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                log.Info("Application paused. Notifying scene manager.");
                _currentSceneManager?.Pause(true);
            }
            else
            {
                log.Info("Application resumed. Notifying scene manager.");
                _currentSceneManager?.Pause(false);
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
            DataCtrlHelper.OnInitDataCtrlHelper();
            log.Info("DataCtrl initialized.");
        }

        /// <summary>
        /// Switch to a new scene manager instance. Destroys the previous manager.
        /// </summary>
        public SceneManagerBase SwitchSceneManager(SceneManagerBase newSceneManagerInstance)
        {
            if (newSceneManagerInstance == null)
            {
                log.Error("SwitchSceneManager failed: new scene manager is null.");
                return null;
            }

            if (_currentSceneManager != null)
            {
                _currentSceneManager.Exit();
                Destroy(_currentSceneManager.gameObject);
            }

            _currentSceneManager = newSceneManagerInstance;
            _currentSceneManager.Init(this);
            _currentSceneManager.Enter();
            log.Info($"Switched to scene manager: {_currentSceneManager.GetType().Name}");
            return _currentSceneManager;
        }

        /// <summary>
        /// Instantiate a scene manager (from prefab if provided) and switch to it.
        /// </summary>
        public T SwitchSceneManager<T>(T prefab = null) where T : SceneManagerBase
        {
            SceneManagerBase instance = prefab != null
                ? Instantiate(prefab, transform)
                : new GameObject(typeof(T).Name).AddComponent<T>();
            instance.transform.SetParent(transform);
            return SwitchSceneManager(instance) as T;
        }

        /// <summary>
        /// Reset the active scene manager (if any).
        /// </summary>
        public void ResetActiveScene()
        {
            _currentSceneManager?.ResetScene();
        }
    }
}
