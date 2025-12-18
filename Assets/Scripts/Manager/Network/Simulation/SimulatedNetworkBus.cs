using System;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.Manager
{
   
    
    /// <summary>
    /// In-Memory Network Bus for local simulation.
    /// Acts as the 'Internet' between Client and Server logic.
    /// </summary>
    public class SimulatedNetworkBus : INetworkBus
    {
        public event Action<ICommand> OnCommandReceived;
        public event Action<IRpc> OnRpcReceived;
        public event Action<int> OnTickSync;

        public SimulatedNetworkBus()
        {
            Debug.Log("[SimNet] Network Bus Initialized.");
        }

        public void SendCommand<T>(T cmd) where T : ICommand
        {
            // Simulate Latency? For now, instant.
            // In a real sim, we might use Observable.Timer or Coroutine.
            
            Debug.Log($"<color=cyan>[Net:Cmd] {cmd.GetType().Name}</color>");
            OnCommandReceived?.Invoke(cmd);
        }

        public void SendRpc<T>(T rpc) where T : IRpc
        {
            Debug.Log($"<color=magenta>[Net:Rpc] {rpc.GetType().Name}</color>");
            OnRpcReceived?.Invoke(rpc);
        }

        public void SyncTick(int tick)
        {
            // Debug.Log($"[Net:Tick] {tick}");
            OnTickSync?.Invoke(tick);
        }
    }
}
