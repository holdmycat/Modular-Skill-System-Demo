using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientSquad : BaseSquad
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientSquad));

        [Inject]
        public ClientSquad(INetworkBus networkBus, IDataLoaderService dataLoaderService, ICharacterDataRepository characterDataRepository)
        {
            log.Info($"[ClientSquad] Construction");
            
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientSquad] InitAsync");
        }

        public override void InitFromSpawnPayload(byte[] payload)
        {
            base.InitFromSpawnPayload(payload);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientSquad] ShutdownAsync");

            await base.ShutdownAsync();
        }
        
        public override void Tick(int tick)
        {
            
        }
        
        public override void OnUpdate()
        {
            
        }
        
        public class Factory : PlaceholderFactory<ClientSquad> 
        {
            public Factory()
            {
                log.Info($"[ClientSquad.Factory] Construction");
            }
        }
        
    }
}
