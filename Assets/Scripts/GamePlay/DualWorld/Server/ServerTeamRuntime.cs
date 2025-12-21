using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerTeamRuntime : BaseTeamRuntime
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ServerTeamRuntime));
        private IDataLoaderService _dataLoaderService;
        private INetworkBus _networkBus;
        public ServerTeamRuntime(IDataLoaderService dataLoaderService, INetworkBus networkBus)
        {
            _dataLoaderService = dataLoaderService;
            _networkBus = networkBus;
            
            BindId(_dataLoaderService.NextId());//server team
            log.Debug("[ServerTeamRuntime] Constructed.");
        }

        public override void InitTeamRuntime(TeamSpawnPayload payload)
        {
            
            log.Debug("[ServerTeamRuntime] InitTeamRuntime.");
            
            base.InitTeamRuntime(payload);
            
            //construct squad
            ConstructSquads();
            
            // Network spawn is handled by ServerPlayer now via RpcSpawnObject.
            // We do not need to send RpcCreateTeam here anymore.
        }

        protected override void ConstructSquads()
        {
            log.Debug("[ServerTeamRuntime] ConstructSquads.");
        }
    }
}
