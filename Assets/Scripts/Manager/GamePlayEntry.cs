//------------------------------------------------------------
// File: GamePlayEntry.cs
// Created: 2025-12-06
// Purpose: Scene entry point to bootstrap GameClientManager and initial scene manager.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------

using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.Manager
{
    public class GamePlayEntry : MonoBehaviour
    {
        [Header("Bootstrap")]
        [SerializeField] private SceneManagerBase initialSceneManagerPrefab;
        [SerializeField] private GlobalGameConfig globalConfig;

        private GameClientManager _gameClientManager;
        
        static readonly ILog log = LogManager.GetLogger(typeof(GamePlayEntry));
        
        private async void Start()
        {

            if (null != _gameClientManager)
            {
                Destroy(_gameClientManager.gameObject);
                _gameClientManager = null;
            }
            
            var go = new GameObject("GameClientManager");
            _gameClientManager = go.AddComponent<GameClientManager>();
            
            // Ensure DataCtrl and default setup are ready.
            _gameClientManager.EnsureDataCtrl();

            GlobalServices.SetGlobalGameConfig(globalConfig);
            
            // Apply global config for resource loading.
            if (globalConfig != null)
            {
                GlobalServices.ResourceLoader = new ResourceLoader(globalConfig.loadMode);
                log.Info($"Global load mode set to {globalConfig.loadMode}.");
            }
            else
            {
                GlobalServices.ResourceLoader = new ResourceLoader(ResourceLoadMode.Resources);
                log.Warn("Global config missing; defaulting load mode to Resources.");
            }

            // If specified, switch to the provided scene manager; otherwise rely on GameClientManager default.
            if (initialSceneManagerPrefab != null)
            {
                log.Info($"Switching to initial scene manager: {initialSceneManagerPrefab.GetType().Name}");
                await _gameClientManager.SwitchSceneManager(initialSceneManagerPrefab);
                await UniTask.DelayFrame(1);
                Destroy(gameObject);
                return;
            }
            
            log.Warn("initialSceneManagerPrefab is null; GameClientManager will use its default (if any).");
        }
    }
}
