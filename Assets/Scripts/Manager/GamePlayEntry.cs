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

            var clientManager = GameClientManager.Instance ? GameClientManager.Instance : FindObjectOfType<GameClientManager>()
                ?? new GameObject("GameClientManager").AddComponent<GameClientManager>();
            
            if (!GlobalServices.IsAppInitialized)
            {

                // Apply global config for resource loading (only first initialization takes effect).
                if (GlobalServices.ResourceLoader == null && globalConfig != null)
                {
                    GlobalServices.InitResourceLoader(new ResourceLoader(globalConfig.loadMode));
                    log.Info($"Global load mode set to {globalConfig.loadMode}.");
                }
                else if (GlobalServices.ResourceLoader == null)
                {
                    GlobalServices.InitResourceLoader(new ResourceLoader(ResourceLoadMode.Resources));
                    log.Warn("Global config missing; defaulting load mode to Resources.");
                }
                
                await clientManager.InitGameClientManager();
                
                
                
                GlobalServices.SetGlobalGameConfig(globalConfig);
                
               
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
