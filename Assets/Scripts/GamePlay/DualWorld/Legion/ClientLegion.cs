using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientLegion));

        [Inject]
        public ClientLegion(INetworkBus networkBus, IDataLoaderService dataLoaderService)
        {
            log.Info($"[ClientLegion] Construction");
            
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientLegion] InitAsync");
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientLegion] ShutdownAsync");
        }
        
        public override void Tick(int tick)
        {
            
        }
        
        public override void OnUpdate()
        {
            
        }
        
        public class Factory : PlaceholderFactory<ClientLegion> 
        {
            public Factory()
            {
                log.Info($"[ClientLegion.Factory] Construction");
            }
        }
        
    }
}
