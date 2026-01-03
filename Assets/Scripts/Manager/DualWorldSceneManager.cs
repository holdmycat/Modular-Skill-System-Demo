//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.Manager
{
    
    public class DualWorldSceneManager : ISceneManager, IFixedTickable, IDisposable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(DualWorldSceneManager));
        
        private BaseServerManager _serverManager;
        private  BaseClientManager _clientManager;

        private INPRuntimeTreeDataProvider _npRuntimeTreeDataProvider;
        
        private bool _isIntialized;
        
        [Inject]
        public void Construct(BaseServerManager serverManager, BaseClientManager clientManager, INPRuntimeTreeDataProvider runtimeTreeDataProvider)
        {
            _serverManager = serverManager;
            _clientManager = clientManager;
            _npRuntimeTreeDataProvider = runtimeTreeDataProvider;
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
            _clientManager.ShutdownAsync().Forget();
        }

        public async UniTask StartupSequence()
        {
            log.Info("[DualWorldSceneManager] StartupSequence: Initializing Dual World...");

            _isIntialized = false;
            
            //0. load battle tree data
            await _npRuntimeTreeDataProvider.InitializeAsync();
            
            // 1. Init Client First (To Listen for Events)
            await _clientManager.InitAsync();
            
            // 2. Init Server (To Generate Events)
            await _serverManager.InitAsync();

            _isIntialized = true;
            
            log.Info("[DualWorldSceneManager] Dual World Initialized.");
        }

        //physics frame
        public void FixedTick()
        {
            if (!_isIntialized)
                return;
            var delta = UnityEngine.Time.fixedDeltaTime;
            _serverManager.OnFixedUpdate(delta);
            _clientManager.OnFixedUpdate(delta);
        }
    }
}
