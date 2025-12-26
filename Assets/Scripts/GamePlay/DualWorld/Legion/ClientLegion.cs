using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientLegion : BaseLegion
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientLegion));

        private readonly ClientSquad.Factory _factory;

        
        [Inject]
        public ClientLegion(
            ClientSquad.Factory factory, 
            INetworkBus networkBus, IDataLoaderService dataLoaderService, ICharacterDataRepository characterDataRepository)
        {
            log.Info($"[ClientLegion] Construction");
            _factory = factory;
            _listBaseSquads = new List<BaseSquad>();
            _characterDataRepository = characterDataRepository;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientLegion] InitAsync");
        }

        public override void InitFromSpawnPayload(byte[] payload)
        {
            var team = LegionSpawnPayload.Deserialize(payload);
            if (team.LegionId == 0)
            {
                throw new System.InvalidOperationException("[ClientLegion] InitFromSpawnPayload missing legion id.");
            }
            if (team.OwnerNetId == 0)
            {
                throw new System.InvalidOperationException("[ClientLegion] InitFromSpawnPayload missing owner net id.");
            }
            if (team.SquadList == null)
            {
                team.SquadList = new System.Collections.Generic.List<long>();
            }
            log.Info($"[ClientLegion] InitFromSpawnPayload legionId:{team.LegionId}, owner:{team.OwnerNetId}, squads:{team.SquadList?.Count ?? 0}");
            base.InitFromSpawnPayload(payload);
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info($"[ClientLegion] ShutdownAsync");

            foreach (var variable in _listBaseSquads)
            {
                await variable.ShutdownAsync();
            }
            _listBaseSquads.Clear();
            
            await base.ShutdownAsync();
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
