using System.IO;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{
    public class DataLoaderService : IDataLoaderService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataLoaderService));
        private bool _isInitialized = false;

        private ResourceLoader _resourceLoader;
        private IModelRepository _modelRepository;
        private ICharacterDataRepository _characterDataRepository;
        private GlobalGameConfig _globalGameConfig;
        public DataLoaderService(GlobalGameConfig config, ResourceLoader resourceLoader, IModelRepository modelRepository, ICharacterDataRepository characterDataRepository)
        {
            _modelRepository = modelRepository;
            _resourceLoader = resourceLoader;
            _characterDataRepository = characterDataRepository;
            _globalGameConfig = config;
        }
        
        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                log.Warn("[DataLoaderService] Already initialized");
                return;
            }

            log.Info("[DataLoaderService] Starting Data Loading...");
            
            //Load Unit Models
            log.Info("[DataLoaderService] Starting Loading Models...");
            var list = await _resourceLoader.LoadAllAssets<UObject>(ResourceAssetType.HeroModelPrefab);
            _modelRepository.SaveModelAsync(list);
            log.Info("[DataLoaderService] Loading Models complete...");
            
            //Load Unit Attribute Data Supportor
            log.Info("[DataLoaderService] Starting Loading character data...");
            var heroData = await _resourceLoader.LoadAsset<TextAsset>(_globalGameConfig.allCharacterDataPath, ResourceAssetType.AllCharacterData);
            using var bsonReader = new BsonBinaryReader(new MemoryStream(heroData.bytes));
            var heroItems = BsonSerializer.Deserialize<UnitAttributesDataSupportor>(bsonReader);
            await _characterDataRepository.SaveUnitDataSupporterAsync(heroItems);
            log.Info("[DataLoaderService] Loading character data complete...");
            
            _isInitialized = true;
            log.Info("[DataLoaderService] Data Loading complete.");
        }

        
    }
}

