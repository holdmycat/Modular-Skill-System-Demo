using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientCommander : BaseCommander
    {
        private ClientSquad.Factory _squadFactory;
        
        [Inject]
        public ClientCommander(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ClientSquad.Factory squadFactory,
            CommanderContextData contextData,
            ICharacterDataRepository characterDataRepository,
            ShowcaseContext showcaseContext) // Inject ShowcaseContext
        {
            log.Info($"[ClientCommander] Construction");

            _squadFactory = squadFactory;
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            _dataLoaderService = dataLoaderService;
            _contextData = contextData;
            _showcaseContext = showcaseContext;
        }
        
        private UnityEngine.GameObject _debugVisual;
        private UnityEngine.Transform _debugRoot;

        public void SetDebugVisualRoot(UnityEngine.Transform root)
        {
            _debugRoot = root;
            if (_debugVisual != null)
            {
                log.Info($"[ClientCommander] Reparenting Visual {_debugVisual.name} to Root {root.name}");
                _debugVisual.transform.SetParent(_debugRoot);
            }
        }

        public override async void InitAsync()
        {
            log.Info($"[ClientCommander] InitAsync");
            
#if UNITY_EDITOR
            if (_globalGameConfig != null && _globalGameConfig.IsDebugVisualsEnabled && _globalGameConfig.ShowCommanderVisual)
            {
                var prefab = await UnityEngine.Resources.LoadAsync<UnityEngine.GameObject>("Models/DebugCommander") as UnityEngine.GameObject;
                if (prefab != null)
                {
                   log.Info($"[ClientCommander] Instantiating Debug Visual for NetId {NetId}");
                   _debugVisual = UnityEngine.Object.Instantiate(prefab);
                   if (_debugRoot != null)
                   {
                       _debugVisual.transform.SetParent(_debugRoot);
                   }
                   _debugVisual.transform.position = Position;
                   _debugVisual.transform.rotation = Rotation;
                   _debugVisual.name = $"Cmd_{Faction}_{NetId}";
                   log.Info($"[ClientCommander] Debug Visual Created: {_debugVisual.name}");
                }
                else
                {
                    log.Warn("[ClientCommander] Failed to load DebugCommander prefab.");
                }
            }
#endif
        }
        
        protected override void InitializeNumeric()
        {
            log.Info($"[ClientCommander] InitializeNumeric");
            _numericComponent = _numericFactory.CreateCommander(_netId);
            _contextData.SetNumericComponent(_numericComponent as CommanderNumericComponent);
            
            // Register Data to ShowcaseContext (Data Layer)
            if (_showcaseContext != null)
            {
                 _showcaseContext.Register(NetId, _numericComponent, Faction);
            }
        }
        
        public override void OnRpc(IRpc rpc)
        {
            if (rpc is RpcSpawnObject spawnMsg && spawnMsg.Type == NetworkPrefabType.Squad)
            {
                log.Info($"[ClientCommander] Received Squad Spawn RPC: NetId:{spawnMsg.NetId}");
                
                var squadPayload = SquadSpawnPayload.Deserialize(spawnMsg.Payload);
                
                var squadData = _characterDataRepository.GetSlgSquadData(squadPayload.SquadId);
                if (squadData == null)
                {
                    log.Error($"[ClientCommander] Squad Data not found for id {squadPayload.SquadId}");
                    return;
                }

                var unitData = _characterDataRepository.GetSlgUnitData(squadData.UnitId);
                
                var squad = _squadFactory.Create();
                squad.Configure(spawnMsg.NetId, squadData, unitData, squadPayload.Faction, eMPNetPosition.eLocalPlayer);
                // Pass debug root to squad
                squad.SetDebugVisualRoot(_debugRoot);
                
                squad.InitAsync();
                _spawnedSquads.Add(squad);
                RecalculateSquadPositions();
                
                log.Info($"[ClientCommander] Spawned Squad {squad.NetId}");
                
                return;
                return;
            }
            else if (rpc is RpcNotifyClientAllSquadsReady)
            {
                log.Info($"[ClientCommander] Received RpcNotifyClientAllSquadsReady! Can Start Battle.");
                _showStartButton = true;
                return;
            }

            base.OnRpc(rpc);
        }
        
        private bool _showStartButton = false;
        
        private void OnGUI()
        {
            if (_showStartButton)
            {
                 // Center button
                 float width = 200;
                 float height = 60;
                 float x = (UnityEngine.Screen.width - width) / 2;
                 float y = UnityEngine.Screen.height * 0.2f; // Top 20%
                 
                 if (UnityEngine.GUI.Button(new UnityEngine.Rect(x, y, width, height), "Start Battle"))
                 {
                     log.Info("[ClientCommander] Start Battle Button Clicked. Sending Command.");
                     _networkBus.SendCommand(NetId, new CmdRequestStartBattle { NetId = NetId });
                     _showStartButton = false;
                 }
            }
        }
        
        public override async UniTask  ShutdownAsync()
        {
            log.Info($"[ClientCommander] ShutdownAsync");
            
            if (_debugVisual != null)
            {
                log.Info($"[ClientCommander] Destroying Debug Visual {_debugVisual.name}");
                UnityEngine.Object.Destroy(_debugVisual);
                _debugVisual = null;
            }
            
            // Unregister Data
            if (_showcaseContext != null)
            {
                _showcaseContext.Unregister(NetId);
            }
            
            foreach (var squad in _spawnedSquads)
            {
                await squad.ShutdownAsync();
            }
            _spawnedSquads.Clear();

            await base.ShutdownAsync();
        }
        
        public override void Tick(int tick)
        {
            for (var i = 0; i < _spawnedSquads.Count; i++)
            {
                _spawnedSquads[i].Tick(tick);
            }
        }
        
        public override void OnUpdate()
        {
            if (_globalGameConfig != null && _globalGameConfig.IsDebugVisualsEnabled && _globalGameConfig.ShowCommanderVisual && _debugVisual != null)
            {
                 _debugVisual.transform.position = Position;
                 _debugVisual.transform.rotation = Rotation;
            }

            for (var i = 0; i < _spawnedSquads.Count; i++)
            {
                _spawnedSquads[i].OnUpdate();
            }
        }
        
        public class Factory : PlaceholderFactory<ClientCommander> 
        {
            public Factory()
            {
                log.Info($"[ClientCommander.Factory] Construction");
            }
        }
    }
}
