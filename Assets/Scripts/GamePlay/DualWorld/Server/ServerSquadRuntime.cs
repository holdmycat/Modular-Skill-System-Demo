using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerSquadRuntime : BaseSquadRuntime
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerSquadRuntime));
        private IDataLoaderService _dataLoaderService;
        private INetworkBus _networkBus;

        public ServerSquadRuntime(IDataLoaderService dataLoaderService, INetworkBus networkBus)
        {
            _dataLoaderService = dataLoaderService;
            _networkBus = networkBus;
            
            BindId(_dataLoaderService.NextId()); // Server Squad NetId
            log.Debug($"[ServerSquadRuntime] Constructed. NetId: {NetId}");
        }

        public override void InitSquadRuntime(SquadSpawnPayload payload)
        {
            base.InitSquadRuntime(payload);
            log.Debug($"[ServerSquadRuntime] InitSquadRuntime. SquadId: {_squadId}, TeamNetId: {_teamNetId}");
        }
    }
}
