using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerLegion));
        
        private readonly ServerSquad.Factory _factory;
        
        [Inject]
        public ServerLegion(
            ServerSquad.Factory factory, 
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ICharacterDataRepository characterDataRepository)
        {
            log.Info($"[ServerLegion] Construction");
            _listBaseSquads = new List<BaseSquad>();
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerLegion] InitAsync");

            foreach (var variable in _squadList)
            {
                log.Info($"[ServerLegion] InitAsync, squadId:" + variable);

                var slgSquadData = _characterDataRepository.GetSlgSquadData(variable);
                
                var baseSquad = _factory.Create();

                var squadNetId = _dataLoaderService.NextId();
                
                baseSquad.Configure(squadNetId, slgSquadData, true);

                baseSquad.InitAsync();
                
                
                

            }
            
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerLegion] ShutdownAsync");
            foreach (var variable in _listBaseSquads)
            {
                await variable.ShutdownAsync();
            }
            _listBaseSquads.Clear();
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
