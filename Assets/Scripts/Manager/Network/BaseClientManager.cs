using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Ebonor.GamePlay;
using UnityEngine;

namespace Ebonor.Manager
{
    public abstract class BaseClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseClientManager));

        protected ClientRoomManager _clientRoomManager;
        
        
        public async UniTask InitAsync()
        {
            log.Info("[BaseClientManager] Initializing...");
            _clientRoomManager.InitAsync();
            await OnInitAsync();
        }

        protected abstract UniTask OnInitAsync();


        private void Update()
        {
            // Frame-based updates for visuals
            if (null != _clientRoomManager)
            {
                _clientRoomManager.OnUpdate();
            }
        }
        
        public virtual async UniTask ShutdownAsync()
        {
            //await _clientRoomManager.ShutdownAsync();
            
            log.Info("[BaseClientManager] Shutting down...");
            
        }
    }
}
