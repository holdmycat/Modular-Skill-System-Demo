using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientCommander : BaseCommander
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientCommander));
        
        private ClientLegion.Factory _legionFactory;
        
        private readonly CommanderNumericComponent.Factory _commanderNumericFactory;
        
        private ShowcaseContext _showcaseContext; // Injected Data Context
        
        [Inject]
        public ClientCommander(
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService, 
            ClientLegion.Factory legionFactory, 
            CommanderNumericComponent.Factory commanderNumericFactory,
            CommanderContextData contextData,
            ShowcaseContext showcaseContext) // Inject ShowcaseContext
        {
            log.Info($"[ClientCommander] Construction");

            _networkBus = networkBus;
            
            _legionFactory = legionFactory;
            
            _commanderNumericFactory = commanderNumericFactory;
            
            _dataLoaderService = dataLoaderService;
            
            _contextData = contextData;
            
            _showcaseContext = showcaseContext;
        }
        
        private CommanderContextData _contextData;
        
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
            
            if (data.Bootstrap == null)
            {
                throw new System.InvalidOperationException("[ClientCommander] InitFromSpawnPayload received null bootstrap.");
            }

            _legionId = data.LegionId;
            
            // Populate Context (Write Once)
            _contextData.SetContext(false,_legionId, data.Bootstrap);
            
            // Helper access check
            // var f = _contextData.Faction; 
            
            log.Info($"[ClientCommander] InitFromSpawnPayload commanderNetId:{NetId}, legionId:{_legionId}");

            Configure(data.Bootstrap);
        }
        
        protected override void InitializeNumeric()
        {
            _numericComponent = _commanderNumericFactory.Create();
            
            // Register Data to ShowcaseContext (Data Layer)
            if (_showcaseContext != null)
            {
                 _showcaseContext.Register(NetId, _numericComponent);
            }
        }
        
        public override void OnRpc(IRpc rpc)
        {
            if (rpc is RpcSpawnObject spawnMsg && spawnMsg.Type == NetworkPrefabType.Legion)
            {
                log.Info($"[ClientCommander] Received Legion Spawn RPC: NetId:{spawnMsg.NetId}");
                
                var legionPayload = LegionSpawnPayload.Deserialize(spawnMsg.Payload);
                var legion = _legionFactory.Create();
                _baseLegion = legion;
                
                var squadList = _bootstrapInfo.LegionConfig.SquadIds;
                
                legion.Configure(spawnMsg.NetId, squadList, false);
                legion.InitFromSpawnPayload(spawnMsg.Payload);

                // _networkBus.RegisterSpawns(legion.NetId, legion); // Handled in Configure
                
                legion.InitAsync();
                
                log.Info($"[ClientCommander] Spawned Legion {legion.NetId}");
                return;
            }

            base.OnRpc(rpc);
        }
        
        public override async UniTask  ShutdownAsync()
        {
            log.Info($"[ClientCommander] ShutdownAsync");
            
            // Unregister Data
            if (_showcaseContext != null)
            {
                _showcaseContext.Unregister(NetId);
            }
            
            if (_baseLegion != null)
            {
                _networkBus.UnRegisterSpawns(_baseLegion.NetId, _baseLegion);
                await _baseLegion.ShutdownAsync();
            }

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
