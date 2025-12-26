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
            CommanderContextData contextData)
        {
            log.Info($"[ServerSquad] Construction");
            
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            
            _faction = contextData.Faction;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerSquad] InitAsync");
            
          
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
