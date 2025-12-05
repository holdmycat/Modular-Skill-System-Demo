//------------------------------------------------------------
// File: LayerMaskSerializer.cs
// Created: 2025-11-29
// Purpose: BSON serializer for Unity LayerMask values.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
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

            // Serialize LayerMask as an integer
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
                var name = reader.ReadName(); // If ReadName cannot be used, fallback to ReadString
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

            // Deserialize integer back into a LayerMask
            return new LayerMask { value = layerMaskValue };
        }
    }
}