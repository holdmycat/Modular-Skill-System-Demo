// ---------------------------------------------
// Script: ClientRoomManager.cs
// Purpose: Client-side room coordinator that listens for RPC spawn/destroy messages and wires up local actors.
// ---------------------------------------------

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Zenject;

namespace Ebonor.GamePlay
{

    //network rpc
    public partial class ClientRoomManager : BaseRoomManager
    {
        public override void OnRpc(IRpc rpc)
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
            
            switch (msg.Type)
            {
                case NetworkPrefabType.Player:
                {
                    var data = CommanderSpawnPayload.Deserialize(msg.Payload);
                    if (data.Equals(default(CommanderSpawnPayload)))
                    {
                        throw new System.InvalidOperationException("[ClientCommander] received empty payload.");
                    }
            
                    if (data.Bootstrap == null)
                    {
                        throw new System.InvalidOperationException("[ClientCommander] received null bootstrap.");
                    }
                    
                    var baseCommander = _factory.Create();
                    baseCommander.Configure(data.Bootstrap, false, msg.NetId);
                    baseCommander.InitAsync();
                    
                    log.Info($"[ClientRoomManager] Successfully Spawned {baseCommander.GetType().Name} [NetId:{msg.NetId}]");
                    
                    _listBaseCommander.Add(baseCommander);
                    
                    break;
                }
                default:
                    log.Error($"[ClientRoomManager] Unknown or Handled-by-Commander Spawn Type: {msg.Type}");
                    return;
            }
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
        private readonly List<BaseCommander> _listBaseCommander = new List<BaseCommander>();
        private ClientCommander.Factory _factory; 
        
        [Inject]
        public void Construct(ClientCommander.Factory factory, INetworkBus networkBus, ICharacterDataRepository characterDataRepository)
        {
            log.Info($"[ClientRoomManager] Construct");
            _factory = factory;
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//client room manager
            _networkBus.RegisterSpawns(NetId, this, false);
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientRoomManager] InitAsync - Listening on Static NetId: {NetId}");
            // Register listener for the Bootstrap Channel (NetId 1)
            //_networkBus.RegisterRpcListener(NetId, OnRpcReceived);
            _networkBus.OnTickSync += Tick;
        }
        
        public override void OnUpdate()
        {
            // Update loop if needed
            foreach (var variable in _listBaseCommander)
            {
                variable.OnUpdate();
            }
          
        }

        public override void Tick(int tick)
        {
            // Tick sync if needed
            foreach (var variable in _listBaseCommander)
            {
                variable.Tick(tick);
            }
        }
        
        public override async UniTask ShutdownAsync()
        {
            foreach (var variable in _listBaseCommander)
            {
                await variable.ShutdownAsync();
            }
            //await _baseCommander.ShutdownAsync();
            
            // if (_networkBus != null)
            //     _networkBus.UnregisterRpcListener(NetId, OnRpcReceived, false);
            
            log.Info("[ClientRoomManager] ShutdownAsync");
        }
    }
}
