using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerLegion));
        
        private readonly ServerSquad.Factory _factory;
        
        [Inject]
        public ServerLegion(
            ServerSquad.Factory factory, 
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            CommanderContextData contextData)
        {
            log.Info($"[ServerLegion] Construction");
            _listBaseSquads = new List<BaseSquad>();
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _factory = factory;
            // Inject Context
            _legionId = contextData.LegionId;
            _faction = contextData.Faction;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerLegion] InitAsync");

            foreach (var squadId in _squadList)
            {
                log.Info($"[ServerLegion] InitAsync, creating squadId: {squadId}");

                var slgSquadData = _characterDataRepository.GetSlgSquadData(squadId);
                if (slgSquadData == null)
                {
                    log.Error($"[ServerLegion] InitAsync failed to get data for squadId: {squadId}");
                    continue;
                }
                
                var baseSquad = _factory.Create();

                var squadNetId = _dataLoaderService.NextId();
                
                baseSquad.Configure(squadNetId, slgSquadData, true);
                
                _listBaseSquads.Add(baseSquad);
                
                var squadPayload = new SquadSpawnPayload
                {
                    SquadId = squadId,
                    OwnerNetId = _netId,
                    LegionNetId = _netId,
                    Faction = _faction
                }.Serialize();

                SpawnChild(_networkBus, baseSquad, squadPayload, NetworkPrefabType.Squad, true);
            }
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerLegion] ShutdownAsync");
            foreach (var variable in _listBaseSquads)
            {
                await variable.ShutdownAsync();
            }
            _listBaseSquads.Clear();
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this, true);
        }
        
        public class Factory : PlaceholderFactory<ServerLegion> 
        {
            public Factory()
            {
                log.Info($"[ServerLegion.Factory] Construction");
            }
        }
    }
}
