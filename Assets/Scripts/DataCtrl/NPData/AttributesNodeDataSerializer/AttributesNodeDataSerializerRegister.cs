//------------------------------------------------------------
// File: AttributesNodeDataSerializerRegister.cs
// Created: 2025-11-29
// Purpose: Registers BSON class maps and type mappings for attribute node data.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using Ebonor.DataCtrl;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public static class AttributesNodeDataSerializerRegister
    {
        private static readonly Dictionary<string, Type> _typeMap;
        public static Dictionary<string, Type> TypeMap => _typeMap;

        static AttributesNodeDataSerializerRegister()
        {
            // Auto-register all subclasses via reflection
            _typeMap = Assembly.GetAssembly(typeof(SlgUnitAttributesNodeData))
                .GetTypes()
                .Where(t => typeof(ICommonAttributeBase).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract)
                .ToDictionary(t => t.Name, t => t);
            
#if UNITY_EDITOR
            var str = $"Registered types via reflection: {TypeMap.Count}:\n";
            int index = 0;
            foreach (var variable in _typeMap)
            {
                str += $"{index}, key:  {variable.Key}\n";
                index++;
            }
            Debug.Log(str);
#endif
        } 
        
    
        public static void RegisterClassMaps()
        {
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(UnitAttributesNodeDataBase)))
            { 
                // BsonSerializer.RegisterSerializer(typeof(UnitAttributesNodeDataBase), new AttributesDataSerializer<UnitAttributesNodeDataBase>());
                BsonClassMap.RegisterClassMap<UnitAttributesNodeDataBase>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true); // supports inheritance
                });
            }
            
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(SlgUnitAttributesNodeData)))
            { 
                // BsonSerializer.RegisterSerializer(typeof(UnitAttributesNodeDataBase), new AttributesDataSerializer<UnitAttributesNodeDataBase>());
                BsonClassMap.RegisterClassMap<SlgUnitAttributesNodeData>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true); // supports inheritance
                });
            }
            
            
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(SlgUnitSquadAttributesNodeData)))
            { 
                // BsonSerializer.RegisterSerializer(typeof(UnitAttributesNodeDataBase), new AttributesDataSerializer<UnitAttributesNodeDataBase>());
                BsonClassMap.RegisterClassMap<SlgUnitAttributesNodeData>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true); // supports inheritance
                });
            }
            
            
            
            // if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(SkillAttributesNodeDataBase)))
            // {
            //     BsonSerializer.RegisterSerializer(typeof(SkillAttributesNodeDataBase), new AttributesDataSerializer<SkillAttributesNodeDataBase>());
            // }
            //
            //
            // if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(DropAttributesNodeDataBase)))
            // {
            //     BsonSerializer.RegisterSerializer(typeof(DropAttributesNodeDataBase), new AttributesDataSerializer<DropAttributesNodeDataBase>());
            // }
            
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(UnityEngine.Vector2)))
            {
                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector2), new VectorSerializer<UnityEngine.Vector2>());
            }
            
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(UnityEngine.Vector3)))
            {
                BsonSerializer.RegisterSerializer(typeof(UnityEngine.Vector3), new VectorSerializer<UnityEngine.Vector3>());
            }
            if (null == BsonSerializer.SerializerRegistry.GetSerializer(typeof(UnityEngine.AnimationCurve)))
            {
                BsonSerializer.RegisterSerializer(typeof(UnityEngine.AnimationCurve), new AnimationCurveSerializer());
            }
            
        }
    }
}
