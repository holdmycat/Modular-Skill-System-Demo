using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerLegion));
        
        [Inject]
        public ServerLegion(INetworkBus networkBus, IDataLoaderService dataLoaderService)
        {
            log.Info($"[ServerLegion] Construction");

            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerLegion] InitAsync");
            
            
            
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerLegion] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this, true);
        }
        
        public class Factory : PlaceholderFactory<ServerLegion> 
        {
            public Factory()
            {
                log.Info($"[ServerLegion.Factory] Construction");
            }
        }
        
    }
}
