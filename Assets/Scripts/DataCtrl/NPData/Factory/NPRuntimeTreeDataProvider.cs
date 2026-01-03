using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using MongoDB.Bson.Serialization;
using UnityEngine;
using Zenject;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Provides NP_DataSupportor for a given tree id/type (data loading/cache to be implemented elsewhere).
    /// </summary>
    public class NPRuntimeTreeDataProvider : INPRuntimeTreeDataProvider
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(NPRuntimeTreeDataProvider));
        private bool _isInitialized = false;
        private ResourceLoader _resourceLoader;
        private  int _counter = 1;

        private readonly Dictionary<KeyValuePair<RuntimeTreeType, long>, NP_DataSupportor> _dicRuntimeTree = new Dictionary<KeyValuePair<RuntimeTreeType, long>, NP_DataSupportor>();

        [Inject]
        public NPRuntimeTreeDataProvider(ResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
        }
        
        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                log.Warn("[NPRuntimeTreeDataProvider] Already initialized");
                return;
            }

            log.Info("[NPRuntimeTreeDataProvider] Starting Data Loading...");
            
            //squad behavour trees
            await LoadAndSaveRuntimeTree(RuntimeTreeType.SlgSquadBehavour, ResourceAssetType.AllSquadBehavour);
            
            _isInitialized = true;
            log.Info("[NPRuntimeTreeDataProvider] Data Loading complete.");

            async UniTask LoadAndSaveRuntimeTree(RuntimeTreeType treeType, ResourceAssetType resourceAssetType)
            {
                
                var listSquadBehavour = await  _resourceLoader.LoadAllAssets<TextAsset>(resourceAssetType);
                foreach (var variable in listSquadBehavour)
                {
                    if (variable.bytes.Length == 0)
                    {
                        log.ErrorFormat("[NPRuntimeTreeDataProvider] battle graph :{0} has no data", variable.name);
                        continue;
                    }
                
                    try
                    {
                        log.Info($"[NPRuntimeTreeDataProvider] 反序列化{treeType}行为树:{variable.name}开始");
                        NP_DataSupportor mnNpDataSupportor = BsonSerializer.Deserialize<NP_DataSupportor>(variable.bytes);
                        log.Info($"[NPRuntimeTreeDataProvider] 反序列化{treeType}行为树:{variable.name}完成");
                        var keyPair = new KeyValuePair<RuntimeTreeType, long>(treeType,
                            mnNpDataSupportor.NpDataSupportorBase.NPBehaveTreeDataId);
                        if (_dicRuntimeTree.ContainsKey(keyPair))
                        {
                            log.Error($"[NPRuntimeTreeDataProvider] Fatal error, 反序列化{treeType}行为树装载字典:{variable.name}已经存在");
                            continue;
                        }
                        _dicRuntimeTree[keyPair] = mnNpDataSupportor;
                    
                        log.Info($"[NPRuntimeTreeDataProvider] 反序列化{treeType}行为树装载字典:{variable.name}完成");
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("[NPRuntimeTreeDataProvider] 反序列化{0}行为树:{1}发生异常, e:{2}", treeType, variable.name, e);
                    }
               
                }
            }
        }
        
        public NP_DataSupportor GetData(long rootId, RuntimeTreeType treeType)
        {
            if (_dicRuntimeTree.TryGetValue(new KeyValuePair<RuntimeTreeType, long>(treeType, rootId), out var tree))
            {
                return tree;
            }
            return null;
        }
    }
}
