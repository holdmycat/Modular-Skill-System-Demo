using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.GamePlay
{
    public class RoomManagerService : MonoBehaviour, IRoomManagerService
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(RoomManagerService));
        
        private CharacterEntity _characterEntity;
        
        private CharacterRuntimeData _characterRuntimeData;
        
        [Inject] 
        protected GlobalGameConfig _globalGameConfig;

        [Inject]
        protected IEntityFactory _entitySpawnService;

        [Inject]
        protected ICharacterDataRepository _characterDataRepository;
        
        public async UniTask CreateRoomAndAddPlayer()
        {
            if (null == _characterEntity)
            {
                _characterRuntimeData = new CharacterRuntimeData(GlobalServices.NextId(), _globalGameConfig.defaultPlayerHeroId, true);
                
                var spawnPos = Vector3.zero; 
                var spawnRot = Quaternion.identity;
                
                var player = await _entitySpawnService.SpawnGameEntity<PlayerEntity>(_characterRuntimeData, spawnPos, spawnRot, transform);
                
                if (player != null)
                {
                    _characterEntity = player;
                    
                    log.Debug($"[RoomManagerService] Player spawned and initialized: {player.name}");
                }
            }
        }
    }
}
