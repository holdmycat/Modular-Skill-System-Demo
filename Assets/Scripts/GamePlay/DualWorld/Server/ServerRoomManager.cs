using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ServerRoomManager : NetworkBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerRoomManager));
        
        private readonly INetworkBus _networkBus;
        private readonly IPlayerDataProvider _playerDataProvider;
        private readonly ITeamIdGenerator _teamIdGenerator;
        private readonly ICharacterDataRepository _characterDataRepository;
        
        private IDataLoaderService _dataLoaderService;
        private BaseCommander _baseCommander;
        
        private readonly ServerCommander.Factory _factory; 
        
        [Inject]
        public ServerRoomManager(
            ServerCommander.Factory factory, 
            IDataLoaderService dataLoaderService, 
            INetworkBus networkBus, 
            IPlayerDataProvider playerDataProvider, 
            ITeamIdGenerator teamIdGenerator, 
            ICharacterDataRepository characterDataRepository)
        {
            _factory = factory;
            _networkBus = networkBus;
            _dataLoaderService = dataLoaderService;
            _playerDataProvider = playerDataProvider;
            _teamIdGenerator = teamIdGenerator;
            _characterDataRepository = characterDataRepository;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//server room manager
            log.Debug("[ServerRoomManager] Constructed (Static NetId: 1).");
        }
        
        public override void InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");
            
            _networkBus.RegisterSpawns(NetId, this, true);
            
            _baseCommander = _factory.Create(); 
            
            var spawnPayload = Array.Empty<byte>(); 
            
            _baseCommander.InitAsync();
            
             _networkBus.SendRpc(new RpcSpawnObject 
             { 
                 Type = NetworkPrefabType.Player,
                 NetId = _baseCommander.NetId,
                 Payload = spawnPayload
             });
             
        }

        public override void Tick(int tick)
        {
        
        }

        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");

            await _baseCommander.ShutdownAsync();
            
            _networkBus.SendRpc(new RpcDestroyObject 
            {
                NetId = NetId
            });
            
        }
    }
}
