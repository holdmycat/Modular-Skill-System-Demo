using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{

    //network rpc
    public partial class ClientRoomManager : MonoBehaviour, IRoomManager
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
                    // Player might not need payload, or minimal payload
                    var clientPlayer = new ClientPlayer(_networkBus, msg.NetId);
                    clientPlayer.InitAsync().Forget();
                    newActor = clientPlayer;
                    break;
                }
                    
                case NetworkPrefabType.Team:
                {
                    // Deserialize payload for Team
                    var teamPayload = NetworkSerializer.Deserialize<TeamSpawnPayload>(msg.Payload);
                    if (teamPayload.SquadList == null) teamPayload.SquadList = new List<long>();
                    
                    var clientTeam = new ClientTeamRuntime(msg.NetId);
                    clientTeam.InitTeamRuntime(teamPayload);
                    newActor = clientTeam;
                    
                    // Link to Owner Actor (Player/AI)
                    var ownerActor = _networkBus.GetSpawnedOrNull(teamPayload.OwnerNetId) as BaseActor;
                    if (ownerActor != null)
                    {
                        ownerActor.SetTeam(clientTeam);
                        log.Info($"[ClientRoomManager] Linked Team {msg.NetId} to Owner {teamPayload.OwnerNetId}");
                    }
                    else
                    {
                        log.Warn($"[ClientRoomManager] Failed to link Team {msg.NetId} to Owner {teamPayload.OwnerNetId} (Owner not found)");
                    }
                    break;
                }

                // Add Squad case later if needed
                // case NetworkPrefabType.Squad: ...
                    
                default:
                    log.Error($"[ClientRoomManager] Unknown Spawn Type: {msg.Type}");
                    return;
            }

                
            log.Info($"[ClientRoomManager] Successfully Spawned {newActor.GetType().Name} [NetId:{msg.NetId}]");
                
            if (newActor != null)
            {
                // Register to the Bus (Single Source of Truth)
                _networkBus.RegisterSpawns(msg.NetId, newActor);
                
                if (msg.Type == NetworkPrefabType.Player)
                {
                    _localPlayer = newActor as BaseActor;
                }
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
    public partial class ClientRoomManager : MonoBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientRoomManager));
        
        private INetworkBus _networkBus;
        private BaseActor _localPlayer;

        // Composition: Network Handle
        private NetworkIdHandle _netHandle;
        public uint NetId => _netHandle.NetId;
        public void BindId(uint netid) => _netHandle.BindId(netid);

        [Inject]
        public void Construct(INetworkBus networkBus)
        {
            _networkBus = networkBus;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//client room manager
        }
        
        public async UniTask InitAsync()
        {
            log.Info($"[ClientRoomManager] InitAsync - Listening on Static NetId: {NetId}");
            
            // Register listener for the Bootstrap Channel (NetId 1)
            _networkBus.RegisterRpcListener(NetId, OnRpcReceived);
            _networkBus.OnTickSync += Tick;
        }
        
        public void OnUpdate()
        {
            // Update loop if needed
        }

        public void Tick(int tick)
        {
            // Tick sync if needed
        }
        
        public async UniTask ShutdownAsync()
        {
            log.Info("[ClientRoomManager] ShutdownAsync");
            
            if (_networkBus != null)
                _networkBus.UnregisterRpcListener(NetId, OnRpcReceived);
        }
    }
}
