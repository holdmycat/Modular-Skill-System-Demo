using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientCommander : BaseCommander
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientCommander));
        
        [Inject]
        public ClientCommander(INetworkBus networkBus, IDataLoaderService dataLoaderService)
        {
            log.Info($"[ClientCommander] Construction");

            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientCommander] InitAsync");
        }

        public override void InitFromSpawnPayload(byte[] payload)
        {
            var data = CommanderSpawnPayload.Deserialize(payload);
            if (data.Equals(default(CommanderSpawnPayload)))
            {
                throw new System.InvalidOperationException("[ClientCommander] InitFromSpawnPayload received empty payload.");
            }

            if (data.CommanderNetId != 0 && data.CommanderNetId != NetId)
            {
                throw new System.InvalidOperationException($"[ClientCommander] NetId mismatch: payload {data.CommanderNetId} vs bound {NetId}");
            }

            if (data.Bootstrap == null)
            {
                throw new System.InvalidOperationException("[ClientCommander] InitFromSpawnPayload received null bootstrap.");
            }

            _legionId = data.LegionId;
            log.Info($"[ClientCommander] InitFromSpawnPayload commanderNetId:{data.CommanderNetId}, legionId:{_legionId}");

            Configure(data.Bootstrap);

            base.InitFromSpawnPayload(payload);
        }
        
        public override async UniTask  ShutdownAsync()
        {
            log.Info($"[ClientCommander] ShutdownAsync");
            await base.ShutdownAsync();
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
