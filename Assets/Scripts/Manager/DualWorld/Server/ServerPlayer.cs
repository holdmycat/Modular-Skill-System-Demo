using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Ebonor.Manager
{
    /// <summary>
    /// Wraps server-side player state so we can hang all player data and logic off a single object.
    /// </summary>
    internal sealed class ServerPlayer
    {
        public PlayerBootstrapInfo BootstrapInfo { get; }
        public ServerFaction Faction { get; }
        private readonly ITeamIdGenerator _teamIdGenerator;

        public ServerPlayer(PlayerBootstrapInfo bootstrapInfo, INetworkBus networkBus, ITeamIdGenerator teamIdGenerator)
        {
            BootstrapInfo = bootstrapInfo;
            _teamIdGenerator = teamIdGenerator;
            var seed = BootstrapInfo.TeamConfig.Seed;
            var _teamIdComp = new TeamIdComponents(
                seed.Usage,
                seed.ScenarioId,
                seed.Faction,
                seed.Slot,
                seed.Variant
            );
            Faction = new ServerFaction(bootstrapInfo.FactionId, networkBus, _teamIdGenerator, _teamIdComp);
        }

        public void InitializeTeams()
        {
            if (BootstrapInfo.TeamConfig == null)
                return;
            
           
            Faction.CreateTeam();
            
          
            // TODO: squad creation RPC when protocol supports it.
        }

        public void Tick(int tick)
        {
            Faction.Tick(tick);
        }

        public UniTask ShutdownAsync()
        {
            // Placeholder for per-player cleanup/hooks.
            return UniTask.CompletedTask;
        }
    }
}
