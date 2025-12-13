using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{
    public class DataLoaderService : IDataLoaderService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataLoaderService));
        private bool _isInitialized = false;

        private ResourceLoader _resourceLoader;
        private IModelRepository _modelRepository;
        
        public DataLoaderService(ResourceLoader resourceLoader, IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
            _resourceLoader = resourceLoader;
        }
        
        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                log.Warn("[DataLoaderService] Already initialized");
                return;
            }

            log.Info("[DataLoaderService] Starting Data Loading...");
            
            log.Info("[DataLoaderService] Starting Loading Models...");
            var list = await _resourceLoader.LoadAllAssets<UObject>(ResourceAssetType.HeroModelPrefab);
            await _modelRepository.SaveModelAsync(list);
            log.Info("[DataLoaderService] Loading Models complete...");
            
            log.Info("[DataLoaderService] Starting Loading character data...");
            log.Info("[DataLoaderService] Loading character data complete...");
            
            _isInitialized = true;
            log.Info("[DataLoaderService] Data Loading complete.");
            
            await UniTask.CompletedTask;
        }

        
    }
}

