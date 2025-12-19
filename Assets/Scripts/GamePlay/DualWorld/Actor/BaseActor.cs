using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;

namespace Ebonor.GamePlay
{
    /// <summary>
    /// Wraps server-side player state so we can hang all player data and logic off a single object.
    /// </summary>
    internal abstract class BaseActor : INetworkBehaviour
    {
        public uint NetId => _netId;
        protected FactionType FractionType => _fractionType;
        
        protected ITeamIdGenerator _teamIdGenerator;
        protected INetworkBus _networkBus;
        protected uint _netId;
        protected IDataLoaderService _dataLoaderService;
        protected FactionType _fractionType;
        protected TeamConfigDefinition _teamConfigDefinition;
        protected long _teamId;
        public abstract void InitializeTeam();
        
        public abstract void Tick(int tick);

        public abstract UniTask ShutdownAsync();
    }
}
