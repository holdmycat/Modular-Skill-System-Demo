using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    public sealed class PlayerBootstrapInfo
    {
        public string PlayerId { get; }
        public FactionType FactionId { get; }
        public TeamConfigDefinition TeamConfig { get; }
        
        public PlayerBootstrapInfo(string playerId, FactionType factionId, TeamConfigDefinition teamConfig)
        {
            PlayerId = playerId;
            FactionId = factionId;
            TeamConfig = teamConfig;
        }
    }
    
    public interface IPlayerDataProvider
    {
        IEnumerable<PlayerBootstrapInfo> GetPlayers();
    }
}
