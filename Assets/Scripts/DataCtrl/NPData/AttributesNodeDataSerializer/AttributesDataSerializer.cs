using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;


namespace Ebonor.DataCtrl
{
    public class AttributesDataSerializer<T> : SerializerBase<T> where T : ICommonAttributeBase
    {
        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartDocument();

            string discriminator = null;
            ICommonAttributeBase instance = null;

            // 读取文档内容，寻找 typeStr 字段以确定具体类型
            while (reader.State != BsonReaderState.EndOfDocument)
            {
                reader.ReadBsonType();
                if (reader.State == BsonReaderState.Name)
                {
                    var fieldName = reader.ReadName();
                    if (fieldName == "typeStr")
                    {
                        discriminator = reader.ReadString();
                        break;
                    }
                    else
                    {
                        // 跳过其他字段
                        reader.SkipValue();
                    }
                }
            }

            if (discriminator == null)
            {
                throw new BsonSerializationException("No discriminator found.");
            }

            if (!AttributesNodeDataSerializerRegister.TypeMap.TryGetValue(discriminator, out var type))
            {
                throw new BsonSerializationException($"Unknown discriminator value '{discriminator}'");
            }

            instance = (ICommonAttributeBase)Activator.CreateInstance(type);

            // 继续读取剩余的文档内容
            while (reader.State != BsonReaderState.EndOfDocument)
            {
                reader.ReadBsonType();
                if (reader.State == BsonReaderState.Name)
                {
                    var fieldName = reader.ReadName();
                    var member = instance.GetType().GetMember(fieldName,
                        MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();

                    if (member != null)
                    {
                        if (member is FieldInfo field)
                        {
                            DeserializeFieldOrProperty(reader, field.FieldType, val => field.SetValue(instance, val));
                        }
                        else if (member is PropertyInfo property && property.CanRead && property.CanWrite)
                        {
                            DeserializeFieldOrProperty(reader, property.PropertyType, val => property.SetValue(instance, val));
                        }
                    }
                    else
                    {
                        // 如果字段或属性没有找到，跳过其值
                        reader.SkipValue();
                    }
                }
            }

            reader.ReadEndDocument();
            return (T)instance;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            var writer = context.Writer;
            writer.WriteStartDocument();

            // 写入类型信息
            writer.WriteName("typeStr");
            writer.WriteString(value.GetType().Name);

            // 序列化字段和属性
            var members = value.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);

            foreach (var member in members)
            {
                writer.WriteName(member.Name);
                if (member is FieldInfo field)
                {
                    SerializeFieldOrProperty(writer, field.GetValue(value));
                }
                else if (member is PropertyInfo property && property.CanRead && property.CanWrite)
                {
                    SerializeFieldOrProperty(writer, property.GetValue(value));
                }
            }

            writer.WriteEndDocument();
        }
        
        private void SerializeFieldOrProperty(IBsonWriter writer, object value)
        {
            if (value is IDictionary dictionary)
            {
                writer.WriteStartDocument();
                foreach (DictionaryEntry entry in dictionary)
                {
                    var key = entry.Key?.ToString();
                    writer.WriteName(key);
                    BsonSerializer.Serialize(writer, entry.Value?.GetType() ?? typeof(object), entry.Value);
                }
                writer.WriteEndDocument();
            }
            else
            {
                BsonSerializer.Serialize(writer, value?.GetType() ?? typeof(object), value);
            }
        }

        private void DeserializeFieldOrProperty(IBsonReader reader, Type memberType, Action<object> setValue)
        {
            if (typeof(IDictionary).IsAssignableFrom(memberType))
            {
                var dictionaryInstance = (IDictionary)Activator.CreateInstance(memberType);
                
                    reader.ReadStartDocument();
                while (reader.State != BsonReaderState.EndOfDocument)
                {
                    reader.ReadBsonType();
                    if (reader.State == BsonReaderState.Name)
                    {
                        var key = reader.ReadName();
                        var keyType = memberType.GetGenericArguments()[0];
                        var valueType = memberType.GetGenericArguments()[1];

                        object keyInstance;
                        if (keyType.IsEnum)
                        {
                            keyInstance = Enum.Parse(keyType, key);
                        }
                        else
                        {
                            keyInstance = Convert.ChangeType(key, keyType);
                        }

                        var valueInstance = BsonSerializer.Deserialize(reader, valueType);
                        dictionaryInstance.Add(keyInstance, valueInstance);
                    }
                }
                reader.ReadEndDocument();
                setValue(dictionaryInstance);
            }
            else
            {
                var fieldValue = BsonSerializer.Deserialize(reader, memberType);
                setValue(fieldValue);
            }
        }
   
    }
}