using System.IO;
using System.Threading;
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
        private IUIAtlasRepository _uiAtlasRepository;
        private GlobalGameConfig _globalGameConfig;
        private  int _counter = 1;
        public DataLoaderService(GlobalGameConfig config, ResourceLoader resourceLoader, IModelRepository modelRepository, ICharacterDataRepository characterDataRepository, IUIAtlasRepository uiAtlasRepository)
        {
            _modelRepository = modelRepository;
            _resourceLoader = resourceLoader;
            _characterDataRepository = characterDataRepository;
            _uiAtlasRepository = uiAtlasRepository;
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
            
            // //Load Unit Attribute Data Supportor
            // log.Info("[DataLoaderService] Starting Loading character data...");
            // var heroData = await _resourceLoader.LoadAsset<TextAsset>(_globalGameConfig.allCharacterDataPath, ResourceAssetType.AllCharacterData);
            // using var bsonReader = new BsonBinaryReader(new MemoryStream(heroData.bytes));
            // var heroItems = BsonSerializer.Deserialize<UnitAttributesDataSupportor>(bsonReader);
            // await _characterDataRepository.SaveUnitDataSupporterAsync(heroItems);
            // log.Info("[DataLoaderService] Loading character data complete...");
            
            //Load slg unit Attribute Data Supporter - SlgUnitAttributesDataSupportor
            log.Info("[DataLoaderService] Starting Loading SLG Unit data...");
            var slgUnitData = await _resourceLoader.LoadAsset<TextAsset>(_globalGameConfig.allSlgUnitDataPath, ResourceAssetType.AllCharacterData);
            if (slgUnitData != null)
            {
                using var slgUnitReader = new BsonBinaryReader(new MemoryStream(slgUnitData.bytes));
                var slgUnitItems = BsonSerializer.Deserialize<SlgUnitAttributesDataSupportor>(slgUnitReader);
                await _characterDataRepository.SaveSlgUnitDataSupporterAsync(slgUnitItems);
                log.Info("[DataLoaderService] Loading SLG Unit data complete...");
            }
            else
            {
                log.Warn($"[DataLoaderService] Failed to load SLG Unit Data at path: {_globalGameConfig.allSlgUnitDataPath}");
            }
            
            
            //Load slg squad Attribute Data Supporter - SlgUnitSquadAttributesDataSupportor
            log.Info("[DataLoaderService] Starting Loading SLG Squad data...");
            var slgSquadData = await _resourceLoader.LoadAsset<TextAsset>(_globalGameConfig.allSlgSquadDataPath, ResourceAssetType.AllCharacterData);
            if (slgSquadData != null)
            {
                using var slgSquadReader = new BsonBinaryReader(new MemoryStream(slgSquadData.bytes));
                var slgSquadItems = BsonSerializer.Deserialize<SlgUnitSquadAttributesDataSupportor>(slgSquadReader);
                await _characterDataRepository.SaveSlgSquadDataSupporterAsync(slgSquadItems);
                log.Info("[DataLoaderService] Loading SLG Squad data complete...");
            }
            else
            {
                log.Warn($"[DataLoaderService] Failed to load SLG Squad Data at path: {_globalGameConfig.allSlgSquadDataPath}");
            }
            
            //Load slg commander Attribute Data Supporter - SlgUnitSquadAttributesDataSupportor
            log.Info("[DataLoaderService] Starting Loading SLG Commander data...");
            var commanderSquadData = await _resourceLoader.LoadAsset<TextAsset>(_globalGameConfig.allSlgCommanderDataPath, ResourceAssetType.AllCharacterData);
            if (commanderSquadData != null)
            {
                using var slgCommanderReader = new BsonBinaryReader(new MemoryStream(commanderSquadData.bytes));
                var slgCommanderItems = BsonSerializer.Deserialize<SlgCommanderAttributesDataSupportor>(slgCommanderReader);
                await _characterDataRepository.SaveSlgCommanderDataSupporterAsync(slgCommanderItems);
                log.Info("[DataLoaderService] Loading SLG Commander data complete...");
            }
            else
            {
                log.Warn($"[DataLoaderService] Failed to load SLG Commander Data at path: {_globalGameConfig.allSlgCommanderDataPath}");
            }
            
            //Load Unit Models
            log.Info("[DataLoaderService] Starting Loading Models...");
            var list = await _resourceLoader.LoadAllAssets<UObject>(ResourceAssetType.HeroModelPrefab);
            _modelRepository.SaveModelAsync(list);
            log.Info("[DataLoaderService] Loading Models complete...");
            
            // Load UI Atlases
            log.Info("[DataLoaderService] Loading UI Atlases...");
            var iconAtlas = await _resourceLoader.LoadAsset<UnityEngine.U2D.SpriteAtlas>(ConstData.UIATLAS_CHARACTERICON, ResourceAssetType.UIAtlas);
            //var commonAtlas = await _resourceLoader.LoadAsset<UnityEngine.U2D.SpriteAtlas>("ui_common", ResourceAssetType.UIAtlas);
            
            _uiAtlasRepository.SaveAtlas(ConstData.UIATLAS_CHARACTERICON, iconAtlas);
            //_uiAtlasRepository.SaveAtlas("ui_common", commonAtlas);
            log.Info("[DataLoaderService] Loading UI Atlases complete.");
            
            
            _isInitialized = true;
            log.Info("[DataLoaderService] Data Loading complete.");
        }
        
        /// <summary>Get the next unique actor ID (uint, starts at 2).</summary>
        /// 1 is reserved for room manager
        public uint NextId()
        {
            return unchecked((uint)Interlocked.Increment(ref _counter));
        }

        
    }
}

