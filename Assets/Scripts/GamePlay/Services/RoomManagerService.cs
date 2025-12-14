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
        
        [Inject] 
        protected GlobalGameConfig _globalGameConfig;

        [Inject]
        protected EntitySpawnService _entitySpawnService;

        [Inject]
        protected ICharacterDataRepository _characterDataRepository;
        
        public async UniTask CreateRoomAndAddPlayer()
        {
            if (null == _characterEntity)
            {
                var characterData = new CharacterRuntimeData(_globalGameConfig.defaultPlayerHeroId, true);
                
                // 1. Get Character Attributes to find the Prefab Name
                var unitAttr = _characterDataRepository.GetUnitAttribteData(characterData._numericId);
                if (unitAttr == null)
                {
                    log.ErrorFormat($"[RoomManagerService] Failed to find unit attributes for ID: {characterData._numericId}");
                    return;
                }
                
                // 2. Spawn Player Entity (Logic Shell + Model)
                // Using a default spawn position for now, can be configured later.
                var spawnPos = Vector3.zero; 
                var spawnRot = Quaternion.identity;
                
                var player = await _entitySpawnService.SpawnGameEntity<PlayerEntity>(unitAttr.UnitAvatar, spawnPos, spawnRot);
                
                if (player != null)
                {
                    _characterEntity = player;
                    
                    // 3. Load Data (Numeric Component)
                    await player.LoadDataAsync<PlayerActorNumericComponent>(characterData);
                    
                    log.Error($"[RoomManagerService] Player spawned and initialized: {player.name}");
                }
            }
        }
    }
}
