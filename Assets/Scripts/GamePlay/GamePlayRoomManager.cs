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
        private PlayerActorInstance _playerActorInstance;
        public PlayerActorInstance PlayerActorInstance => _playerActorInstance;
        
        public async UniTask<ActorInstanceBase> LoadPlayer()
        {
            if (null == _playerActorInstance)
            {
                _playerActorInstance = await LoadCharacter<PlayerActorInstance>();

                // Fall back to hero id 0 when config is not provided (edit-mode tests).
                var defaultHeroId = GlobalServices.GlobalGameConfig != null
                    ? GlobalServices.GlobalGameConfig.defaultPlayerHeroId
                    : 0;

                var characterData = new CharacterRuntimeData(defaultHeroId, true);
                await _playerActorInstance.LoadAsync<PlayerActorNumericComponent>(characterData);
                return _playerActorInstance;
            }
            
            log.Error("Fatal error, _playerActorInstance not null");
            
            return null;
        }
        
        private async UniTask<T> LoadCharacter<T>() where T :ActorInstanceBase
        {
            var go = new GameObject(typeof(T).Name);
            GOHelper.ResetLocalGameObject(gameObject, go, true);
            var result = go.AddComponent<T>();
            return result;
        }
        
        private async UniTask UnLoadPlayer()
        {
            if (_playerActorInstance == null)
            {
                return;
            }

            await _playerActorInstance.UnloadAsync();
            _playerActorInstance = null;
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
