using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Wraps server-side player state so we can hang all player data and logic off a single object.
    /// </summary>
    internal sealed class ServerPlayer : BaseActor
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerPlayer));
        
        public ServerPlayer(TeamConfigDefinition teamConfigDefinition, FactionType type, IDataLoaderService dataLoaderService,  INetworkBus networkBus, ITeamIdGenerator teamIdGenerator)
        {
            _networkBus = networkBus;

            _dataLoaderService = dataLoaderService;

            _teamIdGenerator = teamIdGenerator;
            
            BindId(_dataLoaderService.NextId());//server player
            
            _fractionType = type;

            _teamConfigDefinition = teamConfigDefinition;

            var seed = _teamConfigDefinition.Seed;

            _teamId = teamIdGenerator.GenerateTeamId(new TeamIdComponents(
                seed.Usage,
                seed.ScenarioId,
                seed.Faction,
                seed.Slot,
                seed.Variant
            ));
            
            log.Debug("[ServerPlayer] Constructed.");
            
         
            
        }

        public override async UniTask InitAsync()
        {
            log.Debug("[ServerPlayer] InitializeTeam.");
            
            if (null != _baseTeamRuntime)
            {
                log.Error("[ServerPlayer] Fatal error _baseTeamRuntime is not null.");
                return;
            }
            
            _baseTeamRuntime = new ServerTeamRuntime(_dataLoaderService, _networkBus);

            var teamPayload = new TeamSpawnPayload
            {
                Faction = _fractionType, 
                TeamId = _teamId, 
                OwnerNetId = this.NetId,
                SquadList = _teamConfigDefinition.SquadIds
            };
            
            _baseTeamRuntime.InitTeamRuntime(teamPayload);
            
            _networkBus.SendRpc( 
                new RpcSpawnObject
                {
                    Type = NetworkPrefabType.Team,
                    NetId = _baseTeamRuntime.NetId,
                    Payload = teamPayload.Serialize()
                });
        }

        public override void Tick(int tick)
        {
            
        }

        public override UniTask ShutdownAsync()
        {
            // Placeholder for per-player cleanup/hooks.
            log.Debug("[ServerPlayer] ShutdownAsync.");
            return UniTask.CompletedTask;
        }
    }
}
