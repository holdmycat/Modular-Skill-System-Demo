using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerCommander : BaseCommander
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerCommander));

        private readonly ServerLegion.Factory _factory;

        private readonly CommanderNumericComponent.Factory _commanderNumericFactory;
        
        [Inject]
        public ServerCommander(
            ServerLegion.Factory factory, 
            CommanderNumericComponent.Factory commanderNumericFactory,
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService,
            ILegionIdGenerator legionIdGenerator,
            CommanderContextData contextData)
        {
            log.Info($"[ServerCommander] Construction");

            _factory = factory;

            _commanderNumericFactory = commanderNumericFactory;
            
            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;

            _legionIdGenerator = legionIdGenerator;

            _contextData = contextData;
        }
        
        private CommanderContextData _contextData;
        
        public override void Configure(CommanderBootstrapInfo bootstrapInfo)
        {
            // base.Configure(bootstrapInfo); // MOVED TO END
            
            // Manual set for local logic usage if needed, though we use param 'bootstrapInfo'
            _bootstrapInfo = bootstrapInfo; 
            
            if (_bootstrapInfo == null || _bootstrapInfo.LegionConfig == null)
            {
                log.Error("[ServerCommander] Configure failed: bootstrap info missing.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: bootstrap info missing.");
            }

            if (_bootstrapInfo.LegionConfig.Seed == null)
            {
                log.Error("[ServerCommander] Configure failed: LegionConfig.Seed is null. This is a critical data error.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: LegionConfig.Seed is null.");
            }
            
            _seed = _bootstrapInfo.LegionConfig.Seed;
            var netId = _dataLoaderService.NextId();
            
            _legionId = _legionIdGenerator.Next(netId); // gameplay id
            
            // Populate Context Data (Write Once)
            _contextData.SetContext(true, _legionId, _bootstrapInfo);
            
            // Now call base to trigger Numeric Init (which depends on Context)
            base.Configure(bootstrapInfo);

            BindId(netId);
            _networkBus.RegisterSpawns(NetId, this, true);
        }
        
        public override void InitAsync()
        {
            log.Info($"[ServerCommander] InitAsync");

            if (NetId == 0)
            {
                log.Error("[ServerCommander] InitAsync aborted: commander not configured (NetId==0).");
                return;
            }
            
            _baseLegion = _factory.Create();

            var squadList = _bootstrapInfo.LegionConfig.SquadIds;
            
            var legionNetId = _dataLoaderService.NextId(); // network id (uint)
            _baseLegion.Configure(legionNetId, squadList, true);

            var legionPayloadBytes = new LegionSpawnPayload
            {
                LegionId = (long)_legionId,
                SquadList = _bootstrapInfo?.LegionConfig?.SquadIds ?? new System.Collections.Generic.List<long>(),
                Faction = _seed.Faction,
                OwnerNetId = NetId
            }.Serialize();

            SpawnChild(_networkBus, _baseLegion, legionPayloadBytes, NetworkPrefabType.Legion, true);
            
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerCommander] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this,true);

            await _baseLegion.ShutdownAsync();
        }
        
        protected override void InitializeNumeric()
        {
            _numericComponent = _commanderNumericFactory.Create();
        }
        
        public class Factory : PlaceholderFactory<ServerCommander> 
        {
            public Factory()
            {
                log.Info($"[ServerCommander.Factory] Construction");
            }
        }
    }
}
