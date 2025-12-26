using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    public class ClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientManager));

        private ClientRoomManager _clientRoomManager;
        
        [Inject]
        public void Construct(ClientRoomManager roomManager)
        {
            _clientRoomManager = roomManager;
            GOHelper.ResetLocalGameObject(gameObject,_clientRoomManager.gameObject,true, 1f);
            log.Info("[ClientManager] Constructed (Injected).");
        }
        
        public void InitAsync()
        {
            log.Info("[ClientManager] Initializing...");
            _clientRoomManager.InitAsync();
            
            // Assuming ClientManager is a MonoBehaviour, it will naturally have Update() called by Unity
            // But if we want controlled updates, we could do it here.
        }

        private void Update()
        {
            // Frame-based updates for visuals
            if (null != _clientRoomManager)
            {
                _clientRoomManager.OnUpdate();
            }
        }

        public async UniTask ShutdownAsync()
        {
            await _clientRoomManager.ShutdownAsync();
            
            log.Info("[ClientManager] Shutting down...");
            
        }
    }
}
