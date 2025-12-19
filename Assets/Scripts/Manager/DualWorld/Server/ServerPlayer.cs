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

        public ServerPlayer(PlayerBootstrapInfo bootstrapInfo, INetworkBus networkBus)
        {
            BootstrapInfo = bootstrapInfo;
            Faction = new ServerFaction(bootstrapInfo.FactionId, networkBus);
        }

        public void InitializeTeams()
        {
            foreach (var teamId in BootstrapInfo.TeamIds)
            {
                Faction.CreateTeam(teamId);
            }
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
