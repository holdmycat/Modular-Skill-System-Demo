
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerCommander : BaseCommander
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerCommander));

        private readonly IDataLoaderService _dataLoaderService;
        
        private readonly INetworkBus _networkBus;
        
        [Inject]
        public ServerCommander(INetworkBus networkBus, IDataLoaderService dataLoaderService)
        {
            log.Info($"[ServerCommander] Construction");

            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;

            BindId(_dataLoaderService.NextId());
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerCommander] InitAsync");
            _networkBus.RegisterSpawns(NetId, this, true);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");
            _networkBus.SendRpc(new RpcDestroyObject 
            {
                NetId = NetId
            });
            
        }
        
        
        public class Factory : PlaceholderFactory<ServerCommander> 
        {
            public Factory()
            {
                log.Info($"[ServerCommander.Factory] Construction");
            }
        }
    }
}
