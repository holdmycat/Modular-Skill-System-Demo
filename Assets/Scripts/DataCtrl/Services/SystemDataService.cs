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
            // Basic Types (guard individually; do not early-return to ensure NP node maps are registered)
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Int))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Int));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Bool))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Bool));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Float))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Float));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_String))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_String));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_UInt))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_UInt));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Vector3))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Vector3));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_Long))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_Long));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Long))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Long));
            if (!BsonClassMap.IsClassMapRegistered(typeof(NP_BBValue_List_Byte))) BsonClassMap.LookupClassMap(typeof(NP_BBValue_List_Byte));

            // Custom Serializers (guarded to avoid duplicate registration)
            RegisterSerializerOnce(typeof(System.Numerics.Vector2), new StructBsonSerialize<System.Numerics.Vector2>());
            RegisterSerializerOnce(typeof(System.Numerics.Vector3), new StructBsonSerialize<System.Numerics.Vector3>());
            RegisterSerializerOnce(typeof(VTD_Id), new StructBsonSerialize<VTD_Id>());
            RegisterSerializerOnce(typeof(VTD_EventId), new StructBsonSerialize<VTD_EventId>());
            RegisterSerializerOnce(typeof(UnityEngine.Keyframe), new StructBsonSerialize<UnityEngine.Keyframe>());
            RegisterSerializerOnce(typeof(UnityEngine.Color), new StructBsonSerialize<UnityEngine.Color>());
            
            RegisterSerializerOnce(typeof(UnityEngine.AnimationCurve), new AnimationCurveSerializer());
            RegisterSerializerOnce(typeof(UnityEngine.Vector2), new VectorSerializer<UnityEngine.Vector2>());
            RegisterSerializerOnce(typeof(UnityEngine.Vector3), new VectorSerializer<UnityEngine.Vector3>());
            RegisterSerializerOnce(typeof(Gradient), new GradientSerializer());
            RegisterSerializerOnce(typeof(LayerMask), new LayerMaskSerializer());

            // Generated Registers
            AttributesNodeDataSerializerRegister.RegisterClassMaps();
            NPActionSerializerRegister.RegisterClassMaps();
            NPNodeDataSerializerRegister.RegisterClassMaps();
            GeneratedTypeRegistry.RegisterAllBsonClassMaps();
        }
        private static void RegisterSerializerOnce(System.Type type, IBsonSerializer serializer)
        {
            try
            {
                // Will throw if not registered; if returns a serializer, skip registration
                var _ = BsonSerializer.SerializerRegistry.GetSerializer(type);
                return;
            }
            catch
            {
                // not registered; continue to register
            }
            BsonSerializer.RegisterSerializer(type, serializer);
        }
    }
}
