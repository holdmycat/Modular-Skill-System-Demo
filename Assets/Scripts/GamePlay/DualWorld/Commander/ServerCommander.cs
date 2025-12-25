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
        
        [Inject]
        public ServerCommander(
            ServerLegion.Factory factory, 
            INetworkBus networkBus, 
            IDataLoaderService dataLoaderService,
            ILegionIdGenerator legionIdGenerator)
        {
            log.Info($"[ServerCommander] Construction");

            _factory = factory;
            
            _networkBus = networkBus;
            
            _dataLoaderService = dataLoaderService;

            _legionIdGenerator = legionIdGenerator;
        }
        
        public override void Configure(CommanderBootstrapInfo bootstrapInfo)
        {
            base.Configure(bootstrapInfo);
            if (_bootstrapInfo == null || _bootstrapInfo.LegionConfig == null)
            {
                log.Error("[ServerCommander] Configure failed: bootstrap info missing.");
                throw new System.InvalidOperationException("[ServerCommander] Configure failed: bootstrap info missing.");
            }

            _seed = _bootstrapInfo.LegionConfig.Seed ?? new CommanderSeed();
            var commanderNetId = _legionIdGenerator.GetCommanderNetId(_seed);

            BindId(commanderNetId);
            _networkBus.RegisterSpawns(NetId, this, true);

            _legionId = _legionIdGenerator.Next(NetId); // gameplay id (64-bit)
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

            var legionNetId = _dataLoaderService.NextId(); // network id (uint)
            _baseLegion.Configure(legionNetId, _legionId, true);

            _baseLegion.InitAsync();
            
            var legionPayloadBytes = new LegionSpawnPayload
            {
                LegionId = (long)_legionId,
                SquadList = _bootstrapInfo?.LegionConfig?.SquadIds ?? new System.Collections.Generic.List<long>(),
                Faction = _seed.Faction,
                OwnerNetId = NetId
            }.Serialize();

            _networkBus.SendRpc(_netId, new RpcSpawnObject
            {
                Type = NetworkPrefabType.Legion,
                NetId = _baseLegion.NetId,
                Payload = legionPayloadBytes
            });
            
        }
        
        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerCommander] ShutdownAsync");
            await base.ShutdownAsync();
            _networkBus.UnRegisterSpawns(_netId, this,true);

            await _baseLegion.ShutdownAsync();
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
