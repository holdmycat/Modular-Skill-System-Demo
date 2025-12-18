//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.Manager
{
    
    public class ShowcaseSceneManager : ISceneManager, IDisposable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseSceneManager));
        
        private ServerManager _serverManager;
        private  ClientManager _clientManager;

        [Inject]
        public void Construct(ServerManager serverManager, ClientManager clientManager)
        {
            _serverManager = serverManager;
            _clientManager = clientManager;
            log.Debug("[ShowcaseSceneManager] Constructed with Dual World Managers.");
        }

        public ShowcaseSceneManager()
        {
            log.Debug("[ShowcaseSceneManager] Starting Construction (Wait for Inject)");
        }
        
        public void Dispose()
        {
            log.Debug("[ShowcaseSceneManager] Starting Dispose");
            _serverManager.ShutdownAsync().Forget();
            _clientManager.ShutdownAsync().Forget();
        }

        public async UniTask StartupSequence()
        {
            log.Info("[ShowcaseSceneManager] StartupSequence: Initializing Dual World...");

            // 1. Init Client First (To Listen for Events)
            await _clientManager.InitAsync();

            // 2. Init Server (To Generate Events)
            await _serverManager.InitAsync();
            
            log.Info("[ShowcaseSceneManager] Dual World Initialized.");
        }
        
        private async UniTask LoadRoom()
        {
             // Deprecated legacy method, keeping empty to satisfy potential interface requirements temporarily or just removing.
             await UniTask.CompletedTask;
        }
        
    }
    

}
