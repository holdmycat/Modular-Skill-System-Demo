using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientRoomManager : MonoBehaviour, IRoomManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientRoomManager));
        
        private INetworkBus _networkBus;
        [SerializeField] private uint _netId = 1; // default local client id
        public uint NetId => _netId;

        private BaseActor _localPlayer;
        [Inject]
        public void Construct(INetworkBus networkBus)
        {
            _networkBus = networkBus;
        }

        public async UniTask InitAsync()
        {
            log.Info("[ClientRoomManager] InitAsync - Listening for Server Events");
            // PASSIVE: Do NOT create factions yourself. Wait for Server.
            
            _networkBus.RegisterRpcListener(_netId, OnRpcReceived);
            _networkBus.OnTickSync += Tick;
        }

        private void OnRpcReceived(IRpc rpc)
        {
            if (rpc is RpcCreateCharacter createCharacter)
            {   
                _localPlayer = new ClientPlayer(_networkBus, createCharacter.NetId);
                log.Info($"<color=magenta>[Net:Rpc] {this.GetType().Name}</color>");
            }
        }
        
        public void OnUpdate()
        {
         
        }

        public void Tick(int tick)
        {
          
        }
        
        public async UniTask ShutdownAsync()
        {
            log.Info("[ClientRoomManager] ShutdownAsync");
            
            if (_networkBus != null)
                _networkBus.UnregisterRpcListener(_netId, OnRpcReceived);

          
        }
    }
}
