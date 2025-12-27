using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    public abstract class BaseClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseClientManager));

        protected ClientRoomManager _clientRoomManager;
        
        
        public void InitAsync()
        {
            log.Info("[BaseClientManager] Initializing...");
            _clientRoomManager.InitAsync();
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
            
            log.Info("[BaseClientManager] Shutting down...");
            
        }
    }
}
