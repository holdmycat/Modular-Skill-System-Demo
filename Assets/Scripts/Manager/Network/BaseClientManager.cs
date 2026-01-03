using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using Ebonor.GamePlay;
using UnityEngine;
using Ebonor.DataCtrl;
using Zenject;

namespace Ebonor.Manager
{
    public abstract class BaseClientManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BaseClientManager));

        protected ClientRoomManager _clientRoomManager;
        [Inject(Id = ClockIds.Client)] private Clock _clientClock;
        
        public async UniTask InitAsync()
        {
            log.Info("[BaseClientManager] Initializing...");
            _clientRoomManager.InitAsync();
            await OnInitAsync();
        }

        protected abstract UniTask OnInitAsync();
        
        //client render frame
        private void Update()
        {
            // Frame-based updates for visuals
            if (null != _clientRoomManager)
            {
                _clientRoomManager.OnUpdate();
            }
        }

        //client physics frame
        public void OnFixedUpdate(float deltaTime)
        {
            _clientClock?.OnFixedUpdate(deltaTime);
        }
        
        public virtual async UniTask ShutdownAsync()
        {
            //await _clientRoomManager.ShutdownAsync();
            log.Info("[BaseClientManager] Shutting down...");
        }
    }
}
