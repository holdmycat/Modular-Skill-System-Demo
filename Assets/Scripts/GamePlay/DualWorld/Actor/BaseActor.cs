using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Wraps server-side player state so we can hang all player data and logic off a single object.
    /// </summary>
    internal abstract class BaseActor : NetworkBehaviour
    {
        protected FactionType FractionType => _fractionType;
        
        protected ITeamIdGenerator _teamIdGenerator;
        protected INetworkBus _networkBus;
        protected IDataLoaderService _dataLoaderService;
        protected FactionType _fractionType;
        protected TeamConfigDefinition _teamConfigDefinition;
        protected long _teamId;
        protected BaseTeamRuntime _baseTeamRuntime;
        
        public void SetTeam(BaseTeamRuntime team)
        {
            _baseTeamRuntime = team;
        }
        
    }
}
