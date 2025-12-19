using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    public sealed class PlayerBootstrapInfo
    {
        public string PlayerId { get; }
        public FactionType FactionId { get; }
        public IReadOnlyList<int> TeamIds { get; }
        
        public PlayerBootstrapInfo(string playerId, FactionType factionId, IReadOnlyList<int> teamIds)
        {
            PlayerId = playerId;
            FactionId = factionId;
            TeamIds = teamIds;
        }
    }
    
    public interface IPlayerDataProvider
    {
        IEnumerable<PlayerBootstrapInfo> GetPlayers();
    }
}
