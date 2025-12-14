using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Ebonor.GamePlay
{
    public class EntitySpawnService : IEntityFactory
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EntitySpawnService));
        
        private readonly DiContainer _container;
        private readonly ResourceLoader _resourceLoader;

        private readonly ICharacterDataRepository _characterDataRepository;
        
        public EntitySpawnService(DiContainer container, ResourceLoader resourceLoader, ICharacterDataRepository characterDataRepository)
        {
            _container = container;
            _resourceLoader = resourceLoader;
            _characterDataRepository = characterDataRepository;
        }

        public async UniTask<T> SpawnEntityAsync<T>(long unitId, Vector3 position, Quaternion rotation, Transform parent = null) where T : ActorInstanceBase
        {

            var unitAttribteData = _characterDataRepository.GetUnitAttribteData(unitId);

            if (null == unitAttribteData)
            {
                log.ErrorFormat("[EntitySpawnService], Fail to get unitId:{0} from character data repository", unitId);
                return null;
            }
            
            // 1. Load Prefab
            var prefab = await _resourceLoader.LoadAsset<GameObject>(unitAttribteData.UnitAvatar, ResourceAssetType.HeroModelPrefab);
            if (prefab == null)
            {
                log.Error($"Failed to load entity prefab: {unitId}");
                return null;
            }

            // 2. Instantiate via Zenject
            // InstantiatePrefabForComponent ensures:
            // - The GameObject is created
            // - The component T is returned
            // - Dependencies are injected (if T is a MonoBehaviour with [Inject])
            // - If the prefab has a GameObjectContext, it is initialized and parented correctly
            var instance = _container.InstantiatePrefabForComponent<T>(prefab, position, rotation, parent);

            if (instance == null)
            {
                log.Error($"Failed to instantiate entity: {unitId}");
                return null;
            }

            instance.name = $"{prefab.name}_{instance.GetInstanceID()}";
            
            // Optional: Call manual Init if needed, but Zenject [Inject] is preferred.
            
            return instance;
        }

        public void DespawnEntity(ActorInstanceBase entity)
        {
            if (entity == null) return;
            
            // For now, simple Destroy. 
            // In the future, integrate with Zenject MemoryPool if needed.
            Object.Destroy(entity.gameObject);
        }
    }
}
