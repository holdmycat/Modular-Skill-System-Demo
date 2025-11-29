using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class VectorSerializer<T> : SerializerBase<T> where T : struct
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            var writer = context.Writer;
            writer.WriteStartDocument();

            if (value is Vector3 vector3)
            {
                writer.WriteName("type");
                writer.WriteString("Vector3");
                writer.WriteName("x");
                writer.WriteDouble(vector3.x);
                writer.WriteName("y");
                writer.WriteDouble(vector3.y);
                writer.WriteName("z");
                writer.WriteDouble(vector3.z);
            }
            else if (value is Vector2 vector2)
            {
                writer.WriteName("type");
                writer.WriteString("Vector2");
                writer.WriteName("x");
                writer.WriteDouble(vector2.x);
                writer.WriteName("y");
                writer.WriteDouble(vector2.y);
            }
            else
            {
                throw new BsonSerializationException($"Unsupported type: {typeof(T)}");
            }

            writer.WriteEndDocument();
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartDocument();

            string type = null;
            float x = 0, y = 0, z = 0;

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var name = reader.ReadName();
                switch (name)
                {
                    case "type":
                        type = reader.ReadString();
                        break;
                    case "x":
                        x = (float)reader.ReadDouble();
                        break;
                    case "y":
                        y = (float)reader.ReadDouble();
                        break;
                    case "z":
                        z = (float)reader.ReadDouble();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.ReadEndDocument();

            if (type == "Vector3")
            {
                return (T)(object)new Vector3(x, y, z);
            }
            else if (type == "Vector2")
            {
                return (T)(object)new Vector2(x, y);
            }
            else
            {
                throw new BsonSerializationException($"Unsupported type: {type}");
            }
        }
    }
}
