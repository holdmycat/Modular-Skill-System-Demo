// ---------------------------------------------
// Script: SimulatedNetworkBus.cs
// Purpose: In-memory network bus to simulate RPCs/commands between server and client logic during local runs.
// ---------------------------------------------
using System;
using System.Collections.Generic;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.Manager
{
    /// <summary>
    /// In-Memory Network Bus for local simulation.
    /// Acts as the 'Internet' between Client and Server logic.
    /// </summary>
    public class SimulatedNetworkBus : INetworkBus
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(SimulatedNetworkBus));
        
        public event Action<ICommand> OnCommandReceived;
        public event Action<IRpc> OnRpcReceived;
        public event Action<int> OnTickSync;

        private readonly Dictionary<uint, List<Action<ICommand>>> _cmdListeners = new Dictionary<uint, List<Action<ICommand>>>();
        private readonly Dictionary<uint, List<Action<IRpc>>> _rpcListeners = new Dictionary<uint, List<Action<IRpc>>>();

        private readonly Dictionary<uint, List<(INetworkBehaviour behaviour, bool isServer)>> _dicSpawnActors =
            new Dictionary<uint, List<(INetworkBehaviour behaviour, bool isServer)>>();

        
        public SimulatedNetworkBus()
        {
            log.Info("[SimulatedNetworkBus] Network Bus Initialized.");
        }

        public void SendCommand<T>(uint netId, T cmd) where T : ICommand
        {
            // Simulate Latency? For now, instant.
            // In a real sim, we might use Observable.Timer or Coroutine.
            
            Debug.Log($"<color=cyan>[Net:Cmd] {cmd.GetType().Name}</color>");
            if (_cmdListeners.TryGetValue(netId, out var listeners))
            {
                foreach (var listener in listeners.ToArray())
                {
                    listener?.Invoke(cmd);
                }
            }
            else
            {
                // Backward compatibility: broadcast if no explicit listener registered.
                OnCommandReceived?.Invoke(cmd);
            }
        }
        
        public void SendRpc<T>(uint netId, T rpc) where T : IRpc
        {
            log.Info($"<color=magenta>[Net:Rpc] {rpc.GetType().Name}</color>");
            //uint netId = NetworkConstants.ROOM_MANAGER_NET_ID;
            if (_rpcListeners.TryGetValue(netId, out var listeners))
            {
                foreach (var listener in listeners.ToArray())
                {
                    listener?.Invoke(rpc);
                }
            }
            else
            {
                // Backward compatibility: broadcast if no explicit listener registered.
                OnRpcReceived?.Invoke(rpc);
            }
        }

        public void SyncTick(int tick)
        {
            // Debug.Log($"[Net:Tick] {tick}");
            OnTickSync?.Invoke(tick);
        }

        public void RegisterCommandListener(uint netId, Action<ICommand> handler)
        {
            if (!_cmdListeners.TryGetValue(netId, out var list))
            {
                list = new List<Action<ICommand>>();
                _cmdListeners[netId] = list;
            }
            if (!list.Contains(handler))
            {
                list.Add(handler);
            }
        }

        public void UnregisterCommandListener(uint netId, Action<ICommand> handler)
        {
            if (_cmdListeners.TryGetValue(netId, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _cmdListeners.Remove(netId);
                }
            }
        }

        public void RegisterRpcListener(uint netId, Action<IRpc> handler)
        {
            if (!_rpcListeners.TryGetValue(netId, out var list))
            {
                list = new List<Action<IRpc>>();
                _rpcListeners[netId] = list;
            }
            if (!list.Contains(handler))
            {
                list.Add(handler);
            }
        }

        public void UnregisterRpcListener(uint netId, Action<IRpc> handler)
        {
            if (_rpcListeners.TryGetValue(netId, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _rpcListeners.Remove(netId);
                }
            }
        }

        public void RegisterSpawns(uint netId, INetworkBehaviour behaviour, bool isServer = false, bool isAutoRegisterRpc = true)
        {
            if (!_dicSpawnActors.TryGetValue(netId, out var list))
            {
                list = new List<(INetworkBehaviour behaviour, bool isServer)>();
                _dicSpawnActors[netId] = list;
            }

            if (!list.Exists(x => ReferenceEquals(x.behaviour, behaviour)))
            {
                list.Add((behaviour, isServer));
                if (isAutoRegisterRpc && !isServer)
                {
                    // Auto-wire RPC listener
                    RegisterRpcListener(netId, behaviour.OnRpc);
                }
              
                log.InfoFormat("[RegisterSpawns] netId:{0}, count:{1}", netId, list.Count);
            }
            else
            {
                // Strict check: Do not allow duplicate registration
                throw new InvalidOperationException($"[SimulatedNetworkBus] Duplicate registration detected for NetId:{netId}, Type:{behaviour.GetType().Name}. This indicates a logic error.");
            }
        }

        public void UnRegisterSpawns(uint netId, INetworkBehaviour behaviour, bool isServer = false, bool isAutoUnRegisterRpc = true)
        {
            if (_dicSpawnActors.TryGetValue(netId, out var list))
            {
                list.RemoveAll(x => ReferenceEquals(x.behaviour, behaviour));

                if (isAutoUnRegisterRpc)
                {
                    // Auto-unwire RPC listener
                    UnregisterRpcListener(netId, behaviour.OnRpc);
                }
               

                if (list.Count == 0)
                {
                    _dicSpawnActors.Remove(netId);
                }
                return;
            }

            log.WarnFormat("[UnRegisterSpawns] netId:{0} or behaviour not found", netId);
        }

        public INetworkBehaviour GetSpawnedOrNull(uint netId, bool preferServer = false)
        {
            if (!_dicSpawnActors.TryGetValue(netId, out var list) || list.Count == 0)
            {
                throw new InvalidOperationException($"[SimulatedNetworkBus] No spawned behaviours found for NetId:{netId}");
            }

            if (preferServer)
            {
                var serverEntry = list.Find(x => x.isServer);
                if (serverEntry.behaviour != null)
                {
                    return serverEntry.behaviour;
                }

                throw new InvalidOperationException($"[SimulatedNetworkBus] Requested server behaviour for NetId:{netId} but none registered.");
            }

            var clientEntry = list.FindLast(x => !x.isServer);
            if (clientEntry.behaviour != null)
            {
                return clientEntry.behaviour;
            }

            throw new InvalidOperationException($"[SimulatedNetworkBus] Requested client behaviour for NetId:{netId} but none registered.");
        }
        
        
        
    }
}
