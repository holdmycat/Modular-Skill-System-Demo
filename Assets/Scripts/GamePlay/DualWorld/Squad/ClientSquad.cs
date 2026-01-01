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
        public ClientSquad(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            CommanderContextData contextData, ShowcaseContext showcaseContext)
        {
            log.Info($"[ClientSquad] Construction");
            
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            _dataLoaderService = dataLoaderService;
            _showcaseContext = showcaseContext;
            _contextData = contextData;
            _faction = contextData.Faction;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientSquad] InitAsync");
        }

        protected override void InitializeNumeric()
        {
            _numericComponent = _numericFactory.CreateSquad(_netId, _squadUnitAttr);
            
            // Register Data to ShowcaseContext (Data Layer)
            if (_showcaseContext != null)
            {
                _showcaseContext.Register(NetId, _numericComponent, Faction);
            }
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
