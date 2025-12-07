//------------------------------------------------------------
// File: GamePlayRoomManager.cs
// Purpose: Room-level coordinator (bootstrap/cleanup hooks for the active room).
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{

    //load player
    public partial class GamePlayRoomManager : MonoBehaviour
    {
        public async UniTask<ActorInstanceBase> LoadPlayer()
        {
            return null;
        }

        private async UniTask UnLoadPlayer()
        {
          
        }
    }
    
    public partial class GamePlayRoomManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GamePlayRoomManager));

        /// <summary>
        /// Initialize room-level systems (placeholder for future expansion).
        /// </summary>
        public virtual async UniTask OnInitRoomManager()
        {
            log.Info("RoomManager init.");
            //return UniTask.CompletedTask;
        }

        /// <summary>
        /// Cleanup room-level systems (placeholder for future expansion).
        /// </summary>
        public virtual async UniTask OnUnInitRoomManager()
        {
            log.Info("RoomManager uninit.");
            await UnLoadPlayer();
            //return UniTask.CompletedTask;
        }
        
      
    }
}
