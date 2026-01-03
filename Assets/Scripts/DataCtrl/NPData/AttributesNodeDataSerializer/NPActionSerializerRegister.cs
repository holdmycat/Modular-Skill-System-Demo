using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Registers BSON class maps for NP_ClassForStoreAction and its derived actions.
    /// </summary>
    public static class NPActionSerializerRegister
    {
        public static void RegisterClassMaps()
        {
            var baseType = typeof(NP_ClassForStoreAction);

            BsonSerializer.RegisterDiscriminatorConvention(baseType, new HierarchicalDiscriminatorConvention("_t"));

            if (!BsonClassMap.IsClassMapRegistered(baseType))
            {
                var map = new BsonClassMap(baseType);
                map.AutoMap();
                map.SetIsRootClass(true);
                BsonClassMap.RegisterClassMap(map);
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derived = assemblies
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && t != baseType);

            foreach (var type in derived)
            {
                if (BsonClassMap.IsClassMapRegistered(type))
                {
                    continue;
                }

                var map = new BsonClassMap(type);
                map.AutoMap();
                map.SetDiscriminator(type.Name);
                map.SetDiscriminatorIsRequired(true);
                BsonClassMap.RegisterClassMap(map);
            }
        }
    }
}
