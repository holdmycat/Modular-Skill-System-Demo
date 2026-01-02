using Ebonor.Framework;
using MongoDB.Bson.Serialization;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;
#if UNITY_EDITOR
#endif

namespace Ebonor.DataCtrl
{

    //system
    public class DataCtrl : MonoBehaviour
    {
       
        private static readonly ILog log = LogManager.GetLogger(typeof(DataCtrl));
        
    #if UNITY_EDITOR
            [InitializeOnLoadMethod]
            static void OnLoadMPBattleGraphDataV2()
            {
                OnLoadMPBattleGraphData();
            }
    #endif

            private static bool IsInitedBsonClassMap = false;
            static void OnLoadMPBattleGraphData()
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
                
                GeneratedTypeRegistry.RegisterAllBsonClassMaps();
            }

    }
}
