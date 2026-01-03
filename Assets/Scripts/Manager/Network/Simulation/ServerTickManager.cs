using System;
using Ebonor.DataCtrl;
using UnityEngine;
using Zenject;

namespace Ebonor.Manager
{
    public class ServerTickManager : IInitializable, ITickable, IDisposable
    {
        private readonly INetworkBus _networkBus;
        
        // Config
        public float TickRate = 0.1f; // 10Hz
        public int CurrentTick { get; private set; }

        private float _timer;
        private bool _isRunning = false;

        private readonly BaseServerManager _serverManager;

        [Inject]
        public ServerTickManager(INetworkBus networkBus, BaseServerManager serverManager)
        {
            _networkBus = networkBus;
            _serverManager = serverManager;
        }

        public void Initialize()
        {
            Debug.Log("[ServerTick] Initialized.");
            _isRunning = true;
        }

        public void Tick()
        {
            if (!_isRunning) return;

            _timer += Time.deltaTime; // Using Unity Time for sim simplicity
            
            while (_timer >= TickRate)
            {
                _timer -= TickRate;
                RunTick();
            }
        }
        
        private void RunTick()
        {
            CurrentTick++;

            // 1. Logic Update
             _serverManager.Tick(CurrentTick); 
            
            // 2. Sync to Clients
            _networkBus.SyncTick(CurrentTick);
        }
        public void Dispose()
        {
            _isRunning = false;
        }
    }
}
