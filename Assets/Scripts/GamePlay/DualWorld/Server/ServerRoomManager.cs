using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;

namespace Ebonor.GamePlay
{
    public class ServerRoomManager : IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerRoomManager));
        
        private readonly INetworkBus _networkBus;
        private readonly IPlayerDataProvider _playerDataProvider;
        private readonly ITeamIdGenerator _teamIdGenerator;
        private BaseActor _localPlayer;
        
        private IDataLoaderService _dataLoaderService;
        private uint _netId;
        private const uint ClientNetId = 1;
        public uint NetId => _netId;
        public ServerRoomManager(IDataLoaderService dataLoaderService, INetworkBus networkBus, IPlayerDataProvider playerDataProvider, ITeamIdGenerator teamIdGenerator)
        {
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _playerDataProvider = playerDataProvider;
            _teamIdGenerator = teamIdGenerator;
            _netId = dataLoaderService.NextId();
            log.Debug("[ServerRoomManager] Constructed.");
        }
        
        public async UniTask InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");

            _localPlayer = new ServerPlayer(_playerDataProvider.GetPlayers().TeamConfig, FactionType.Player, _dataLoaderService, _networkBus, _teamIdGenerator);
            
            _networkBus.SendRpc(ClientNetId, new RpcCreateCharacter { NetId = _dataLoaderService.NextId() });
            
            _localPlayer.InitializeTeam();
        }

        public void Tick(int tick)
        {
            if (null == _localPlayer)
                return;
            
            _localPlayer.Tick(tick);
        }

        public async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");
            
            
        }
    }
}

