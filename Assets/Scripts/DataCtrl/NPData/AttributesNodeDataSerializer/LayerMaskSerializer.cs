using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class LayerMaskSerializer : SerializerBase<LayerMask>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LayerMask value)
        {
            var writer = context.Writer;
            writer.WriteStartDocument();

            // 将 LayerMask 序列化为一个整数
            writer.WriteName("layerMaskValue");
            writer.WriteInt32(value.value);

            writer.WriteEndDocument();
        }

        public override LayerMask Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartDocument();

            int layerMaskValue = 0;

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var name = reader.ReadName(); // 如果 ReadName 无法使用，可以使用 ReadString
                if (name == "layerMaskValue")
                {
                    layerMaskValue = reader.ReadInt32();
                }
                else
                {
                    reader.SkipValue();
                }
            }

            reader.ReadEndDocument();

            // 将整数反序列化为 LayerMask
            return new LayerMask { value = layerMaskValue };
        }
    }
}