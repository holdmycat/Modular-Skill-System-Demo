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

        static readonly ILog log = LogManager.GetLogger(typeof(GamePlayEntry));
        
        private async void Start()
        {
            var clientManager = gameObject.GetComponent<GameClientManager>() ?? gameObject.AddComponent<GameClientManager>();

            // Ensure DataCtrl and default setup are ready.
            clientManager.EnsureDataCtrl();

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
                await clientManager.SwitchSceneManager(initialSceneManagerPrefab);
                return;
            }
            
            log.Warn("initialSceneManagerPrefab is null; GameClientManager will use its default (if any).");
        }
    }
}
