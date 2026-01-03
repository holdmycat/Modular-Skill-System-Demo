using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerSquad : BaseSquad
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerSquad));
        
        [Inject]
        public ServerSquad(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository,
            INPRuntimeTreeFactory npRuntimeTreeFactory,
            GlobalGameConfig globalGameConfig,
            CommanderContextData contextData)
        {
            log.Info($"[ServerSquad] Construction");
            
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _npRuntimeTreeFactory = npRuntimeTreeFactory;
            _faction = contextData.Faction;
            _globalGameConfig = globalGameConfig;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerSquad] InitAsync");
            
            //create soldiers
            var formation = _squadUnitAttr.Formation;

            var count = _squadUnitAttr.MaxCount;
            
            //我需要知道加载的模型id
            var unitId = _squadUnitAttr.UnitId;

            //我需要知道加载的位置

            //我需要知道阵型




        }
        
        protected override void InitializeNumeric()
        {
            _numericComponent = _numericFactory.CreateSquad(_netId, _squadUnitAttr);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerSquad] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this, true);
        }
        
        public class Factory : PlaceholderFactory<ServerSquad> 
        {
            public Factory()
            {
                log.Info($"[ServerSquad.Factory] Construction");
            }
        }
    }
}
