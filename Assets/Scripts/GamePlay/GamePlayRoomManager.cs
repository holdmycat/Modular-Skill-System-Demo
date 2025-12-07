//------------------------------------------------------------
// File: GamePlayRoomManager.cs
// Purpose: Room-level coordinator (bootstrap/cleanup hooks for the active room).
//------------------------------------------------------------
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.GamePlay
{
    public class GamePlayRoomManager : MonoBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GamePlayRoomManager));

        /// <summary>
        /// Initialize room-level systems (placeholder for future expansion).
        /// </summary>
        public virtual UniTask OnInitRoomManager()
        {
            log.Info("RoomManager init.");
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Cleanup room-level systems (placeholder for future expansion).
        /// </summary>
        public virtual UniTask OnUnInitRoomManager()
        {
            log.Info("RoomManager uninit.");
            return UniTask.CompletedTask;
        }
    }
}
