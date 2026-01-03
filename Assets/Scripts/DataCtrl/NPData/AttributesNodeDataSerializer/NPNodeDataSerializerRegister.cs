using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Registers BSON class maps for NP node data types to support polymorphic deserialization.
    /// </summary>
    public static class NPNodeDataSerializerRegister
    {
        public static void RegisterClassMaps()
        {
            var baseType = typeof(NP_NodeDataBase);

            // Ensure discriminator convention uses _t (matches serialized data)
            BsonSerializer.RegisterDiscriminatorConvention(baseType, new HierarchicalDiscriminatorConvention("_t"));
            if (!BsonClassMap.IsClassMapRegistered(baseType))
            {
                var map = new BsonClassMap(baseType);
                map.AutoMap();
                map.SetIsRootClass(true);
                BsonClassMap.RegisterClassMap(map);
            }

            var assembly = Assembly.GetAssembly(baseType);
            var derivedTypes = assembly
                .GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            foreach (var type in derivedTypes)
            {
                if (BsonClassMap.IsClassMapRegistered(type))
                {
                    continue;
                }

                var map = new BsonClassMap(type);
                map.AutoMap();
                map.SetDiscriminatorIsRequired(true);
                map.SetDiscriminator(type.Name);
                BsonClassMap.RegisterClassMap(map);
            }
        }
    }
}
