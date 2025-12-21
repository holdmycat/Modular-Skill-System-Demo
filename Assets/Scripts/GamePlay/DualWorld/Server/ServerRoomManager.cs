using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerRoomManager : NetworkBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerRoomManager));
        
        private readonly INetworkBus _networkBus;
        private readonly IPlayerDataProvider _playerDataProvider;
        private readonly ITeamIdGenerator _teamIdGenerator;
        private BaseActor _localPlayer;
        
        private IDataLoaderService _dataLoaderService;
       
        public ServerRoomManager(IDataLoaderService dataLoaderService, INetworkBus networkBus, IPlayerDataProvider playerDataProvider, ITeamIdGenerator teamIdGenerator)
        {
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _playerDataProvider = playerDataProvider;
            _teamIdGenerator = teamIdGenerator;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//server room manager
            log.Debug("[ServerRoomManager] Constructed (Static NetId: 1).");
        }
        
        public override async UniTask InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");

            _localPlayer = new ServerPlayer(_playerDataProvider.GetPlayers().TeamConfig, FactionType.Player, _dataLoaderService, _networkBus, _teamIdGenerator);
            
            // Spawn the Player on Client
            // Player doesn't need complex payload yet, or maybe just Faction? 
            // For now sending empty payload or basic info if needed.
            // Using a simple 4-byte payload for NetId verification or similar if needed, but empty is fine for now.
            var spawnPayload = new byte[0]; 
            
            _networkBus.SendRpc(new RpcSpawnObject 
            { 
                Type = NetworkPrefabType.Player,
                NetId = _localPlayer.NetId,
                Payload = spawnPayload
            });
            
            
            await _localPlayer.InitAsync();
            
        }

        public override void Tick(int tick)
        {
            if (null == _localPlayer)
                return;
            
            _localPlayer.Tick(tick);
        }

        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");
            
            
        }
    }
}

