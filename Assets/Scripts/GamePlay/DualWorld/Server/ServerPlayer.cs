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
            
            _netId = _dataLoaderService.NextId() ;
            
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

        public override void InitializeTeam()
        {
            log.Debug("[ServerPlayer] InitializeTeam.");
            
            
            _networkBus.SendRpc(_netId, 
                new RpcCreateTeam
                {
                    FactionId = _fractionType, 
                    TeamId = _teamId, 
                    SquadList = _teamConfigDefinition.SquadIds
                });
            
            
            //construct squad
            
            
            
            
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
