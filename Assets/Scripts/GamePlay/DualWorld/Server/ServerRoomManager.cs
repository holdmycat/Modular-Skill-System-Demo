using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{

    //commander - scene config enemy
    public partial class ServerRoomManager : NetworkBehaviour, IRoomManager
    {
        
    }
    
    
    //commander - player
    public partial class ServerRoomManager : NetworkBehaviour, IRoomManager
    {
        private void InitPlayerCommander()
        {
            _baseCommander = _factory.Create(); 
            
            var bootstrap = _sceneResourceManager?.GetPlayerCommanderBootstrapInfo() ?? _sceneResourceManager?.GetCommanderBootstrapInfo();
            if (bootstrap == null)
            {
                throw new System.InvalidOperationException("[ServerRoomManager] InitPlayerCommander failed: bootstrap is null.");
            }

            _baseCommander.Configure(bootstrap);

            
            var payload = new CommanderSpawnPayload
            {
                LegionId = _baseCommander.LegionId,
                Bootstrap = bootstrap
            };
            var spawnPayload = payload.Serialize();

            _networkBus.SendRpc(NetworkConstants.ROOM_MANAGER_NET_ID, new RpcSpawnObject 
            { 
                Type = NetworkPrefabType.Player,
                NetId = _baseCommander.NetId,
                Payload = spawnPayload
            });
            
            _baseCommander.InitAsync();
            
        }
    }
    
    //system
    public partial class ServerRoomManager : NetworkBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerRoomManager));
        
        private readonly INetworkBus _networkBus;
        private BaseCommander _baseCommander;
        private readonly ServerCommander.Factory _factory; 
        
        [Inject]
        public ServerRoomManager(
            ServerCommander.Factory factory, 
            INetworkBus networkBus, ISceneResourceManager sceneResourceManager)
        {
            _factory = factory;
            _networkBus = networkBus;
            _sceneResourceManager = sceneResourceManager;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//server room manager
            _networkBus.RegisterSpawns(NetId, this, true);
            log.Info("[ServerRoomManager] Constructed (Static NetId: 1).");
        }
        
        public override void InitAsync()
        {
            log.Info("[ServerRoomManager] InitAsync");

            InitPlayerCommander();
        }

        public override void Tick(int tick)
        {
        
        }

        public override async UniTask ShutdownAsync()
        {
            log.Info("[ServerRoomManager] ShutdownAsync");

            _networkBus.UnRegisterSpawns(_netId, this, true,false);
            
            await _baseCommander.ShutdownAsync();

            _networkBus.SendRpc(NetworkConstants.ROOM_MANAGER_NET_ID, new RpcDestroyObject 
            {
                NetId = NetId
            });
        }
    }
}
