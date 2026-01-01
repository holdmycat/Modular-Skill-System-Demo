using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerCommander : BaseCommander
    {
        

        private readonly ServerSquad.Factory _factory;
        
        
        [Inject]
        public ServerCommander(
            ServerSquad.Factory factory,
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService,
            ICharacterDataRepository characterDataRepository, // Inject here
            CommanderContextData contextData)
        {
            log.Info($"[ServerCommander] Construction");

            _factory = factory;
            
            _characterDataRepository = characterDataRepository; // Assign here
            
            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;
            
            _contextData = contextData;
        }


        public override void InitAsync()
        {
            log.Info($"[ServerCommander] InitAsync");

            if (NetId == 0)
            {
                log.Error("[ServerCommander] InitAsync aborted: commander not configured (NetId==0).");
                return;
            }
            
            var squadList = _bootstrapInfo.LegionConfig.SquadIds;
            
            foreach (var squadId in squadList)
            {
                var baseSquad = _factory.Create();
                
                var slgSquadData = _characterDataRepository.GetSlgSquadData(squadId);
                if (slgSquadData == null)
                {
                    log.Error($"[ServerCommander] InitAsync failed to get data for squadId: {squadId}");
                    continue; // Skip if data missing
                }

                var squadNetId = _dataLoaderService.NextId();
                baseSquad.Configure(squadNetId, slgSquadData, true);
                
                _spawnedSquads.Add(baseSquad);
                
                var squadPayload = new SquadSpawnPayload
                {
                    SquadId = squadId,
                    OwnerNetId = _netId, // Owned by Commander now
                    Faction = _seed.Faction
                }.Serialize();
                
                SpawnChild(_networkBus, baseSquad, squadPayload, NetworkPrefabType.Squad, true);
            }
            
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerCommander] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this,true);

            foreach (var squad in _spawnedSquads)
            {
                await squad.ShutdownAsync();
            }
            _spawnedSquads.Clear();
        }
        
        protected override void InitializeNumeric()
        {
            _numericComponent = _numericFactory.CreateCommander(_netId);
        }
        
        public class Factory : PlaceholderFactory<ServerCommander> 
        {
            public Factory()
            {
                log.Info($"[ServerCommander.Factory] Construction");
            }
        }
    }
}
