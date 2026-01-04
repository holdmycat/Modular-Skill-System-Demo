// ---------------------------------------------
// Script: ClientRoomManager.cs
// Purpose: Client-side room coordinator that listens for RPC spawn/destroy messages and wires up local actors.
// ---------------------------------------------

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
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
                    baseCommander.Configure(data.Bootstrap, eMPNetPosition.eLocalPlayer, msg.NetId);
                    
#if UNITY_EDITOR
                    if (_debugVisualsRoot != null)
                    {
                        baseCommander.SetDebugVisualRoot(_debugVisualsRoot.transform);
                    }
#endif
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
            var actor = _networkBus.GetSpawnedOrNull(msg.NetId, eMPNetPosition.eLocalPlayer);
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
        private Clock _clock; 
        
        private GlobalGameConfig _globalGameConfig;
        private UnityEngine.GameObject _debugVisualsRoot;

        [Inject]
        public void Construct(ClientCommander.Factory factory, INetworkBus networkBus, ICharacterDataRepository characterDataRepository, GlobalGameConfig globalGameConfig, [Inject(Id = ClockIds.Client)] Clock clock)
        {
            log.Info($"[ClientRoomManager] Construct");
            _factory = factory;
            _networkBus = networkBus;
            _characterDataRepository = characterDataRepository;
            _globalGameConfig = globalGameConfig;
            _clock = clock;
            BindId(NetworkConstants.ROOM_MANAGER_NET_ID);//client room manager
            _networkBus.RegisterSpawns(NetId, this, eMPNetPosition.eLocalPlayer);
        }
        
        public override void InitAsync()
        {
            log.Info($"[ClientRoomManager] InitAsync - Listening on Static NetId: {NetId}");
            // Register listener for the Bootstrap Channel (NetId 1)
            //_networkBus.RegisterRpcListener(NetId, OnRpcReceived);
            _networkBus.OnTickSync += Tick;
            
#if UNITY_EDITOR
            if (_globalGameConfig != null && _globalGameConfig.IsDebugVisualsEnabled)
            {
                log.Info("[ClientRoomManager] Creating DebugVisualsRoot");
                _debugVisualsRoot = new UnityEngine.GameObject("DebugVisualsRoot");
                _debugVisualsRoot.transform.SetParent(transform);
                _debugVisualsRoot.transform.localPosition = UnityEngine.Vector3.zero;
            }
#endif
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

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (_listBaseCommander == null) return;
            if (_globalGameConfig == null) return;

            foreach (var commander in _listBaseCommander)
            {
                if (commander == null) continue;

                Gizmos.color = UnityEngine.Color.blue;
                Gizmos.DrawWireCube(commander.Position, UnityEngine.Vector3.one);
                
                UnityEditor.Handles.Label(commander.Position + UnityEngine.Vector3.up * 2, $"Cmd [{commander.Faction}]\n{commander.Position}");

                var squads = commander.GetSpawnedSquads(); 
                if (squads != null)
                {
                     foreach (var squad in squads)
                     {
                         if(squad == null) continue;
                         
                         Gizmos.color = UnityEngine.Color.green;
                         UnityEngine.Vector3 size = new UnityEngine.Vector3(_globalGameConfig.SlgSquadDimensions.x, 2f, _globalGameConfig.SlgSquadDimensions.y);
                         Gizmos.DrawWireCube(squad.Position, size);
                         
                         UnityEditor.Handles.Label(squad.Position + UnityEngine.Vector3.up * 2, $"Squad\n{squad.Position}");
                         
                         // Draw Soldiers
                         Gizmos.color = UnityEngine.Color.red;
                         
                         // Need Count. BaseSquad doesn't expose _squadUnitAttr publicly.
                         // But GetSoldierLocalPosition is public and uses internal data if available?
                         // Actually GetSoldierLocalPosition is purely math based on Index + Config.
                         // We need the COUNT.
                         // BaseSquad needs to expose Count.
                         // For now, let's try to get count or use a safe Loop if we can't.
                         // Assuming we modify BaseSquad to expose Count or just iterate 16 for demo.
                         // Let's iterate 16 as default if we can't verify.
                         // Better: Expose 'CurrentSoldierCount' or 'InitialCount' in BaseSquad.
                         // I will add 'GetInitialCount()' to BaseSquad in next step for correctness.
                         int count = squad.GetInitialCount();
                         
                         for(int i=0; i<count; i++)
                         {
                             UnityEngine.Vector3 localPos = squad.GetSoldierLocalPosition(i);
                             UnityEngine.Vector3 worldPos = squad.Position + squad.Rotation * localPos;
                             Gizmos.DrawSphere(worldPos, 0.2f);
                         }
                     }
                }
            }
#endif
        }
    }
}
