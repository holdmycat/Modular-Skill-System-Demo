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
        
        public override void InitAsync()
        {
            log.Info($"[ClientCommander] InitAsync");
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
                
                var squad = _squadFactory.Create();
                squad.Configure(spawnMsg.NetId, squadData, squadPayload.Faction, false);
                squad.InitAsync();
                _spawnedSquads.Add(squad);
                
                log.Info($"[ClientCommander] Spawned Squad {squad.NetId}");
                
                return;
            }

            base.OnRpc(rpc);
        }
        
        public override async UniTask  ShutdownAsync()
        {
            log.Info($"[ClientCommander] ShutdownAsync");
            
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
