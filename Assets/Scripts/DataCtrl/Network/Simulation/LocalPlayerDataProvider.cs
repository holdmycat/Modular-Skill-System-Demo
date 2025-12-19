using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    public class LocalPlayerDataProvider : IPlayerDataProvider
    {
        private readonly List<PlayerBootstrapInfo> _players;

        private GlobalGameConfig _globalGameConfig;
        
        public LocalPlayerDataProvider(GlobalGameConfig config)
        {

            _globalGameConfig = config;
            
            var list = new int[_globalGameConfig.TeamIds.Count];

            int index = 0;
            foreach (var variable in _globalGameConfig.TeamIds)
            {
                list[index] = variable;

                index++;
            }
            
            _players = new List<PlayerBootstrapInfo>
            {
                new PlayerBootstrapInfo("local-player", FactionType.Player, list)
            };
        }
        
        public IEnumerable<PlayerBootstrapInfo> GetPlayers()
        {
            return _players;
        }
    }
}
