using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    public class LocalPlayerDataProvider : IPlayerDataProvider
    {
        private readonly List<PlayerBootstrapInfo> _players;

        public LocalPlayerDataProvider(GlobalGameConfig config)
        {
            var playerTeam = config.PlayerBirthTeamConfigInst?.PlayerTeam ?? new TeamConfigDefinition
            {
                Seed = new TeamIdSeed
                {
                    Usage = TeamUsageType.Player,
                    ScenarioId = "local",
                    Faction = FactionType.Player,
                    Slot = 0,
                    Variant = "default"
                }
            };

            _players = new List<PlayerBootstrapInfo>
            {
                new PlayerBootstrapInfo("local-player", FactionType.Player, playerTeam)
            };
        }
        
        public IEnumerable<PlayerBootstrapInfo> GetPlayers()
        {
            return _players;
        }
    }
}
