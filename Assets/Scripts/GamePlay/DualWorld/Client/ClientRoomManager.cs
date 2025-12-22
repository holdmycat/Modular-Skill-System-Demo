// ---------------------------------------------
// Script: ClientRoomManager.cs
// Purpose: Client-side room coordinator that listens for RPC spawn/destroy messages and wires up local actors.
// ---------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{

    //network rpc
    public partial class ClientRoomManager : BaseRoomManager
    {
        private void OnRpcReceived(IRpc rpc)
        {
            // Only handle Spawn Objects on this channel
            if (rpc is RpcSpawnObject spawnMsg)
            {
                OnSpawnObject(spawnMsg);
            }
            else if (rpc is RpcDestroyObject destroyMsg)
            {
                OnDestroyObject(destroyMsg);
            }
            else
            {
                log.Warn($"[ClientRoomManager] Received unexpected RPC type: {rpc?.GetType().Name}");
            }
        }

        private void OnSpawnObject(RpcSpawnObject msg)
        {
            log.Info($"[ClientRoomManager] Spawn Request: {msg.Type} (NetId:{msg.NetId})");
            
            INetworkBehaviour newActor = null;

            switch (msg.Type)
            {
                case NetworkPrefabType.Player:
                {
                    _baseCommander = _factory.Create();
                    newActor = _baseCommander;
                    newActor.BindId(msg.NetId);
                    newActor.InitFromSpawnPayload(msg.Payload);
                    break;
                }
                default:
                    log.Error($"[ClientRoomManager] Unknown Spawn Type: {msg.Type}");
                    return;
            }

            newActor.InitAsync();
            
            _networkBus.RegisterSpawns(newActor.NetId, newActor);
            
            log.Info($"[ClientRoomManager] Successfully Spawned {newActor.GetType().Name} [NetId:{msg.NetId}]");
        }

        private void OnDestroyObject(RpcDestroyObject msg)
        {
            var actor = _networkBus.GetSpawnedOrNull(msg.NetId);
            if (actor != null)
            {
                 _networkBus.UnRegisterSpawns(msg.NetId, actor);
                 
                 // Perform any cleanup on the actor itself
                 if (actor is { } networkBehaviour)
                 {
                     networkBehaviour.ShutdownAsync().Forget();
                 }
                 
                 log.Info($"[ClientRoomManager] Destroyed object NetId:{msg.NetId}");
            }
            else
            {
                log.Warn($"[ClientRoomManager] Could not find object to destroy NetId:{msg.NetId}");
            }
        }
        
    }
    
    //system
    public partial class ClientRoomManager : BaseRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientRoomManager));
        
        private INetworkBus _networkBus;
        private ICharacterDataRepository _characterDataRepository;
        private BaseCommander _baseCommander;
        
        // Composition: Network Handle
     

        private ClientCommander.Factory _factory; 
        
        [Inject]
        public void Construct(ClientCommander.Factory factory,  INetworkBus networkBus, ICharacterDataRepository characterDataRepository)
        {
            log.Info($"[ClientRoomManager] Construct");
            _factory = factory;
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//client room manager
           
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientRoomManager] InitAsync - Listening on Static NetId: {NetId}");
            // Register listener for the Bootstrap Channel (NetId 1)
            _networkBus.RegisterRpcListener(NetId, OnRpcReceived);
            _networkBus.OnTickSync += Tick;
            _networkBus.RegisterSpawns(NetId, this);
        }
        
        public override void OnUpdate()
        {
            // Update loop if needed
            if (null != _baseCommander)
            {
                _baseCommander.OnUpdate();
            }
        }

        public override void Tick(int tick)
        {
            // Tick sync if needed
            if (null != _baseCommander)
            {
                _baseCommander.Tick(tick);
            }
        }
        
        public override async UniTask ShutdownAsync()
        {
            

            await _baseCommander.ShutdownAsync();
            
            if (_networkBus != null)
                _networkBus.UnregisterRpcListener(NetId, OnRpcReceived);
            
            log.Info("[ClientRoomManager] ShutdownAsync");
        }
    }
}
