//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Ebonor.GamePlay;
using Zenject;

namespace Ebonor.Manager
{
    
    public class DualWorldSceneManager : ISceneManager, IDisposable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(DualWorldSceneManager));
        
        private BaseServerManager _serverManager;
        private  BaseClientManager _clientManager;
        
        [Inject]
        public void Construct(BaseServerManager serverManager, BaseClientManager clientManager)
        {
            _serverManager = serverManager;
            _clientManager = clientManager;
            log.Info("[DualWorldSceneManager] Constructed with Dual World Managers.");
        }
        
        public DualWorldSceneManager()
        {
            log.Info("[DualWorldSceneManager] Starting Construction (Wait for Inject)");
        }
        
        public void Dispose()
        {
            log.Info("[DualWorldSceneManager] Starting Dispose");
            _serverManager.ShutdownAsync().Forget();
        }

        public async UniTask StartupSequence()
        {
            log.Info("[DualWorldSceneManager] StartupSequence: Initializing Dual World...");

            // 1. Init Client First (To Listen for Events)
            _clientManager.InitAsync();
            
            // 2. Init Server (To Generate Events)
            _serverManager.InitAsync();
            
            log.Info("[DualWorldSceneManager] Dual World Initialized.");
        }
    }
}
