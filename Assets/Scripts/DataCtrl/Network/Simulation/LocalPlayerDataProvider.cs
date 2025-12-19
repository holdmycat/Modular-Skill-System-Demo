
namespace Ebonor.DataCtrl
{
    public class LocalPlayerDataProvider : IPlayerDataProvider
    {
        private PlayerBootstrapInfo _player;
        
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
            
            _player = new PlayerBootstrapInfo("local-player", FactionType.Player, playerTeam);
        }
        
        public PlayerBootstrapInfo GetPlayers()
        {
            return _player;
        }
    }
}
