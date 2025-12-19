using System;
using System.Collections.Generic;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
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

        public SimulatedNetworkBus()
        {
            Debug.Log("[SimNet] Network Bus Initialized.");
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
    }
}
