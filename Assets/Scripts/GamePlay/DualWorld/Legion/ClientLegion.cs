using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientLegion));

        private readonly ClientSquad.Factory _factory;
        
        [Inject]
        public ClientLegion(
            ClientSquad.Factory factory, 
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            CommanderContextData contextData)
        {
            log.Info($"[ClientLegion] Construction");
            _factory = factory;
            _listBaseSquads = new List<BaseSquad>();
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            
            // Inject Context
            _legionId = contextData.LegionId;
            _faction = contextData.Faction;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientLegion] InitAsync");
        }

        public override void InitFromSpawnPayload(byte[] payload)
        {
            var team = LegionSpawnPayload.Deserialize(payload);
            if (team.LegionId == 0)
            {
                throw new System.InvalidOperationException("[ClientLegion] InitFromSpawnPayload missing legion id.");
            }
            if (team.OwnerNetId == 0)
            {
                throw new System.InvalidOperationException("[ClientLegion] InitFromSpawnPayload missing owner net id.");
            }
            if (team.SquadList == null)
            {
                log.Error("[ClientLegion] InitFromSpawnPayload failed: SquadList is null in payload.");
                throw new System.InvalidOperationException("[ClientLegion] InitFromSpawnPayload failed: SquadList is null.");
            }
            
            // Faction is already injected via DI, but payload might have overrides? 
            // For now, assume DI is truth for this Commander Context.
            // _faction = team.Faction; 
            
            log.Info($"[ClientLegion] InitFromSpawnPayload legionId:{team.LegionId}, owner:{team.OwnerNetId}, squads:{team.SquadList?.Count ?? 0}, faction:{team.Faction}");
            base.InitFromSpawnPayload(payload);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientLegion] ShutdownAsync");

            foreach (var variable in _listBaseSquads)
            {
                await variable.ShutdownAsync();
            }
            _listBaseSquads.Clear();
            
            await base.ShutdownAsync();
        }
        
        public override void Tick(int tick)
        {
            
        }
        
        public override void OnRpc(IRpc rpc)
        {
            if (rpc is RpcSpawnObject spawnMsg && spawnMsg.Type == NetworkPrefabType.Squad)
            {
                log.Info($"[ClientLegion] Received Squad Spawn RPC: NetId:{spawnMsg.NetId}");

                var squadPayload = SquadSpawnPayload.Deserialize(spawnMsg.Payload);
                var squadData = _characterDataRepository.GetSlgSquadData(squadPayload.SquadId);

                if (squadData == null)
                {
                    log.Error($"[ClientLegion] Failed to find squad data for id: {squadPayload.SquadId}");
                    return;
                }
                
                var squad = _factory.Create();
                squad.Configure(spawnMsg.NetId, squadData, false);
                squad.InitFromSpawnPayload(spawnMsg.Payload);
                squad.InitAsync();
                _listBaseSquads.Add(squad);
                log.Info($"[ClientLegion] Spawned Squad {squad.NetId}");
                return;
            }
            base.OnRpc(rpc);
        }

        public override void OnUpdate()
        {
            
        }
        
        public class Factory : PlaceholderFactory<ClientLegion> 
        {
            public Factory()
            {
                log.Info($"[ClientLegion.Factory] Construction");
            }
        }
        
    }
}
