using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using UnityEditor;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    //all character data
    public partial class DataCtrl : MonoBehaviour
    {
        private Dictionary<long, UnitAttributesNodeDataBase> _dicUnitAttriDatas;
        private Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>> _dicListUnitDatas;
        
        private async UniTask LoadAllCharacterDataAsync(GlobalGameConfig config)
        {
            if (config == null)
            {
                log.Warn("GlobalGameConfig is null; skipping character data load.");
                return;
            }

            _dicUnitAttriDatas ??= new Dictionary<long, UnitAttributesNodeDataBase>();
            _dicUnitAttriDatas.Clear();

            _dicListUnitDatas ??= new Dictionary<eActorModelType, List<UnitAttributesNodeDataBase>>();
            _dicListUnitDatas.Clear();

            for (var i = eActorModelType.eHero; i <= eActorModelType.eEventSkillActor; i++)
            {
                _dicListUnitDatas.Add(i, new List<UnitAttributesNodeDataBase>());
            }
            
            var globalConfig = config.allCharacterDataPath;
            if (GlobalServices.ResourceLoader == null)
            {
                log.Warn("ResourceLoader is null; skipping character data load.");
                return;
            }
            
            var heroData = await GlobalServices.ResourceLoader.LoadAsset<TextAsset>(globalConfig, ResourceAssetType.AllCharacterData);
            if (heroData == null)
            {
                log.Warn($"Character data not found at path: {globalConfig}");
                return;
            }
            using var bsonReader = new BsonBinaryReader(new MemoryStream(heroData.bytes));
            var heroItems = BsonSerializer.Deserialize<UnitAttributesDataSupportor>(bsonReader);
            foreach (var hero in heroItems.UnitAttributesDataSupportorDic.Values)  
            {                    
                _dicUnitAttriDatas.Add(hero.UnitDataNodeId, hero);  
                _dicListUnitDatas[hero.ActorModelType].Add(hero);  
                log.Debug("heroId:" + hero.UnitDataNodeId);
            }
        }

        private void UnLoadAllCharacterDataAsync()
        {
            _dicUnitAttriDatas.Clear();
            _dicListUnitDatas.Clear();
        }

        public UnitAttributesNodeDataBase GetUnitAttributeNodeData(long id)
        {
            if (_dicUnitAttriDatas.TryGetValue(id, out var attr))
            {
                return attr;
            }

            return null;
        }
        
    }
    
    //load all system data
    public partial class DataCtrl : MonoBehaviour
    {
        /// <summary>
        /// Async method to load all system data (BSON registration, etc.)
        /// </summary>
        public async UniTask LoadAllSystemDataAsync(IProgress<float> progress)
        {
            if (IsInitedBsonClassMap)
            {
                progress?.Report(1.0f);
                return;
            }

            progress?.Report(0.1f);
                    
            // Find assemblies
            Assembly _dataCtrl = null;
            Assembly _multiPlayer = null;
            Assembly _manager = null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.Contains(ConstData.AD_DATACTRL))
                {
                    _dataCtrl = assembly;
                }
                else if (assembly.FullName.Contains(ConstData.AD_MULTIPLAYER))
                {
                    _multiPlayer = assembly;
                }
                else if (assembly.FullName.Contains(ConstData.AD_MANAGER))
                {
                    _manager = assembly;
                }
            }
                    
            progress?.Report(0.3f);

            // Perform BSON registration
            // Note: BsonClassMap and other MongoDB types are generally thread-safe for registration 
            // as long as we don't access Unity APIs here.
            OnLoadMPBattleGraphData(_dataCtrl, _multiPlayer, _manager);
                    
            progress?.Report(0.8f);

            // Stage 2: Main thread finalization (if any Unity assets need loading)
            // Currently just finishing up
            progress?.Report(1.0f);
        }


        public async UniTask LoadAllGameDataAsync(IProgress<float> progress)
        {
            
            var globalConfig = GlobalServices.GlobalGameConfig;
            
            //all character data
            await LoadAllCharacterDataAsync(globalConfig);
            progress?.Report(0.1f);
            
            //
            
            //
            
            
            progress?.Report(1.0f);
            
        }
        
    }
    
    //system
    public partial class DataCtrl : MonoBehaviour
    {
       
        private static readonly ILog log = LogManager.GetLogger(typeof(DataCtrl));
        
        private static DataCtrl _inst;
        public static DataCtrl Inst => _inst;
        
        
        public void InitializeDataCtrl()
        {
            _inst = this;
        }
        
        private void OnDestroy()
        {
            UnLoadAllCharacterDataAsync();
            _inst = null;
        }
        
        
    #if UNITY_EDITOR
            [InitializeOnLoadMethod]
            static void OnLoadMPBattleGraphDataV2()
            {
                Assembly _dataCtrl = null;
                Assembly _multiPlayer = null;
                Assembly _manager = null;


                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.FullName.Contains(ConstData.AD_DATACTRL))
                    {
                        _dataCtrl = assembly;
                    }
                    else if (assembly.FullName.Contains(ConstData.AD_MULTIPLAYER))
                    {
                        _multiPlayer = assembly;
                    }
                    else if (assembly.FullName.Contains(ConstData.AD_MANAGER))
                    {
                        _manager = assembly;
                    }
                }

                OnLoadMPBattleGraphData(_dataCtrl, _multiPlayer, _manager);
            }
    #endif

            private static bool IsInitedBsonClassMap = false;
            static void OnLoadMPBattleGraphData(Assembly _dataCtrl, Assembly _multiPlayer, Assembly manager)
            {
                // ConventionPack conventionPack = new ConventionPack {new IgnoreExtraElementsConvention(true)};
                // ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

                if (IsInitedBsonClassMap)
                {
                    return;
                }
                
                IsInitedBsonClassMap = true;
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Int)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_Int));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Bool)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_Bool));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Float)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_Float));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_String)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_String));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_UInt)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_UInt));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Vector3)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_Vector3));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Long)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_Long));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Long)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Long));
                if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Byte)))
                    BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Byte));


                BsonSerializer.RegisterSerializer(typeof(System.Numerics.Vector2),
                    new StructBsonSerialize<System.Numerics.Vector2>());

                BsonSerializer.RegisterSerializer(typeof(System.Numerics.Vector3),
                    new StructBsonSerialize<System.Numerics.Vector3>());

                BsonSerializer.RegisterSerializer(typeof(VTD_Id), new StructBsonSerialize<VTD_Id>());

                BsonSerializer.RegisterSerializer(typeof(VTD_EventId), new StructBsonSerialize<VTD_EventId>());

                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Keyframe),
                    new StructBsonSerialize<UnityEngine.Keyframe>());

                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Color), new StructBsonSerialize<UnityEngine.Color>());

                BsonSerializer.RegisterSerializer(typeof(UnityEngine.AnimationCurve), new AnimationCurveSerializer());

                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector2), new VectorSerializer<UnityEngine.Vector2>());

                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector3), new VectorSerializer<UnityEngine.Vector3>());

                BsonSerializer.RegisterSerializer(typeof(Gradient), new GradientSerializer());

                BsonSerializer.RegisterSerializer(typeof(LayerMask), new LayerMaskSerializer());
                

                AttributesNodeDataSerializerRegister.RegisterClassMaps();
                var types = new List<Type>();
                //types.AddRange(_multiPlayer.GetTypes());
                types.AddRange(_dataCtrl.GetTypes());
                //types.AddRange(manager.GetTypes());

                foreach (Type type in types)
                {
                    if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        continue;
                    }

                    if (type.IsGenericType)
                    {
                        continue;
                    }

                    if (!BsonClassMap.IsClassMapRegistered(type))
                        BsonClassMap.LookupClassMap(type);
                }

                RegisterAllSubClassForDeserialize(types);

                

                // var filePath = Path.Combine(Application.dataPath, ("AssetPackages/BattleGraphData/SkillGraphData/"));
                //
                // DirectoryInfo directory = new DirectoryInfo(filePath);
                // FileInfo[] fileInfos = directory.GetFiles();
                // byte[] mfile = File.ReadAllBytes(fileInfos[0].FullName);
                // NP_DataSupportor MnNpDataSupportor = null;
                // try
                // {
                //     MnNpDataSupportor = BsonSerializer.Deserialize<NP_DataSupportor>(mfile);
                // }
                // catch (Exception e)
                // {
                //     log.Error(e);
                //     return;
                // }
                // Debug.Log("Success Deserialize NP_DataSupportor");
                //log.Debug("Finish Register Bson data types");
            }

            static void RegisterAllSubClassForDeserialize(List<Type> allTypes)
            {
                List<Type> parenTypes = new List<Type>();
                List<Type> childrenTypes = new List<Type>();
                // registe by BsonDeserializerRegisterAttribute Automatically  
                foreach (Type type in allTypes)
                {
                    BsonDeserializerRegisterAttribute[] bsonDeserializerRegisterAttributes =
                        type.GetCustomAttributes(typeof(BsonDeserializerRegisterAttribute), false) as
                            BsonDeserializerRegisterAttribute[];
                    if (bsonDeserializerRegisterAttributes.Length > 0)
                    {
                        parenTypes.Add(type);
                    }

                    BsonDeserializerRegisterAttribute[] bsonDeserializerRegisterAttributes1 =
                        type.GetCustomAttributes(typeof(BsonDeserializerRegisterAttribute), true) as
                            BsonDeserializerRegisterAttribute[];
                    if (bsonDeserializerRegisterAttributes1.Length > 0)
                    {
                        childrenTypes.Add(type);
                    }
                }

                foreach (Type type in childrenTypes)
                {
                    foreach (var parentType in parenTypes)
                    {
                        if (parentType.IsAssignableFrom(type) && parentType != type)
                        {
                            if (!BsonClassMap.IsClassMapRegistered(type))
                            {
                                BsonClassMap.LookupClassMap(type);
                            }
                        }
                    }
                }
            }

            
    }
}
