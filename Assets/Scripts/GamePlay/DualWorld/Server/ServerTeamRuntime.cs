using Ebonor.DataCtrl;
using Ebonor.Framework;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Ebonor.GamePlay
{
    public class ServerTeamRuntime : BaseTeamRuntime
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ServerTeamRuntime));
        private IDataLoaderService _dataLoaderService;
        private INetworkBus _networkBus;
        private readonly Dictionary<long, ServerSquadRuntime> _squadRuntimes = new Dictionary<long, ServerSquadRuntime>();
        private readonly Dictionary<uint, ServerSquadRuntime> _squadRuntimesByNetId = new Dictionary<uint, ServerSquadRuntime>();
        public ServerTeamRuntime(IDataLoaderService dataLoaderService, INetworkBus networkBus)
        {
            _dataLoaderService = dataLoaderService;
            _networkBus = networkBus;
            
            BindId(_dataLoaderService.NextId());//server team
            log.Debug("[ServerTeamRuntime] Constructed.");
        }

        public override void InitTeamRuntime(TeamSpawnPayload payload)
        {
            
            log.Debug("[ServerTeamRuntime] InitTeamRuntime.");
            
            base.InitTeamRuntime(payload);
            
            //construct squad
            ConstructSquads();
        }

        protected override void ConstructSquads()
        {
            log.Debug("[ServerTeamRuntime] ConstructSquads.");

            if (_squadList == null) return;

            foreach (var squadId in _squadList)
            {
                var squadRuntime = new ServerSquadRuntime(_dataLoaderService, _networkBus);
                
                var payload = new SquadSpawnPayload
                {
                    SquadId = squadId,
                    OwnerNetId = _ownerNetId, // Team's owner (Player's NetId)
                    TeamNetId = NetId,      // Team's NetId
                    Faction = _factionType
                };
                
                squadRuntime.InitSquadRuntime(payload);
                
                // Spawn on Client
                _networkBus.SendRpc(new RpcSpawnObject
                {
                    Type = NetworkPrefabType.Squad, 
                    NetId = squadRuntime.NetId,
                    Payload = payload.Serialize()
                });
                
                // Register on server bus for lookup/management
                _networkBus.RegisterSpawns(squadRuntime.NetId, squadRuntime, true);
                
                _squadRuntimes[squadId] = squadRuntime;
                _squadRuntimesByNetId[squadRuntime.NetId] = squadRuntime;
                
                log.Debug($"[ServerTeamRuntime] Spawned Squad {squadId} with NetId {squadRuntime.NetId}");
            }

        }

        public override async UniTask ShutdownAsync()
        {
            await base.ShutdownAsync();
            _squadRuntimes.Clear();
            _squadRuntimesByNetId.Clear();
            
            log.Debug("[ServerTeamRuntime] ShutdownAsync.");
        }
        
    }
}
