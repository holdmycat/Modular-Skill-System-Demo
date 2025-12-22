using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientCommander : BaseCommander
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientCommander));
        
        private readonly IDataLoaderService _dataLoaderService;
        private readonly INetworkBus _networkBus;

        [Inject]
        public ClientCommander(INetworkBus networkBus, IDataLoaderService dataLoaderService)
        {
            log.Info($"[ClientCommander] Construction");
            
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientCommander] InitAsync");
            
            //_networkBus.RegisterSpawns(NetId, this);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientCommander] ShutdownAsync");
        }
        

        public override void Tick(int tick)
        {
            
        }
        
        public override void OnUpdate()
        {
            
        }
        
        public class Factory : PlaceholderFactory<ClientCommander> 
        {
            public Factory()
            {
                log.Info($"[ClientCommander.Factory] Construction");
            }
        }
    }
}
