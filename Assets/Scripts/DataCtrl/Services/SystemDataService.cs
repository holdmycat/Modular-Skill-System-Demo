using Cysharp.Threading.Tasks;
using MongoDB.Bson.Serialization;
using UnityEngine;
using Ebonor.Framework;
using System.IO;

namespace Ebonor.DataCtrl
{
    public class SystemDataService : ISystemDataService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SystemDataService));
        private bool _isInitialized = false;

        public async UniTask InitializeAsync()
        {
            if (_isInitialized) return;

            log.Info("[SystemDataService] Starting BSON registration...");
            
            // Run BSON registration on a background thread if possible, 
            // but BsonClassMap usually needs to be safe. 
            // For safety and simplicity in this refactor, we keep it synchronous but wrapped in UniTask.
            // If it's heavy, we can use UniTask.Run, but be careful with Unity API access inside serializers.
            
            RegisterBsonMaps();

            _isInitialized = true;
            log.Info("[SystemDataService] BSON registration complete.");
            
            await UniTask.CompletedTask;
        }

        private void RegisterBsonMaps()
        {
            // Copied from DataCtrl.OnLoadMPBattleGraphData
            
            if (BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Int))) return; // Safety check

            // Basic Types
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Int))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Int));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Bool))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Bool));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Float))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Float));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_String))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_String));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_UInt))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_UInt));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Vector3))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Vector3));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Long))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Long));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Long))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Long));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Byte))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Byte));

            // Custom Serializers
            BsonSerializer.RegisterSerializer(typeof(System.Numerics.Vector2), new StructBsonSerialize<System.Numerics.Vector2>());
            BsonSerializer.RegisterSerializer(typeof(System.Numerics.Vector3), new StructBsonSerialize<System.Numerics.Vector3>());
            BsonSerializer.RegisterSerializer(typeof(VTD_Id), new StructBsonSerialize<VTD_Id>());
            BsonSerializer.RegisterSerializer(typeof(VTD_EventId), new StructBsonSerialize<VTD_EventId>());
            BsonSerializer.RegisterSerializer(typeof(UnityEngine.Keyframe), new StructBsonSerialize<UnityEngine.Keyframe>());
            BsonSerializer.RegisterSerializer(typeof(UnityEngine.Color), new StructBsonSerialize<UnityEngine.Color>());
            
            BsonSerializer.RegisterSerializer(typeof(UnityEngine.AnimationCurve), new AnimationCurveSerializer());
            BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector2), new VectorSerializer<UnityEngine.Vector2>());
            BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector3), new VectorSerializer<UnityEngine.Vector3>());
            BsonSerializer.RegisterSerializer(typeof(Gradient), new GradientSerializer());
            BsonSerializer.RegisterSerializer(typeof(LayerMask), new LayerMaskSerializer());

            // Generated Registers
            AttributesNodeDataSerializerRegister.RegisterClassMaps();
            GeneratedTypeRegistry.RegisterAllBsonClassMaps();
        }
    }
}
