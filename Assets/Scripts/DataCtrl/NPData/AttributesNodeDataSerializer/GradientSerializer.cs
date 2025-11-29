using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

public class GradientSerializer : SerializerBase<Gradient>
{
    // 反序列化
    public override Gradient Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
        var gradient = new Gradient();
        if (!document.Elements.Any())
            return gradient;

        var colorKeysArray = document["ColorKeys"].AsBsonArray;
        var alphaKeysArray = document["AlphaKeys"].AsBsonArray;

        var colorKeys = colorKeysArray.Select(bsonValue => {
            var doc = bsonValue.AsBsonDocument;
            return new GradientColorKey(
                new Color(
                    (float)doc["R"].ToDouble(),
                    (float)doc["G"].ToDouble(),
                    (float)doc["B"].ToDouble(),
                    (float)doc["A"].ToDouble()
                ),
                (float)doc["Time"].ToDouble()
            );
        }).ToArray();

        var alphaKeys = alphaKeysArray.Select(bsonValue => {
            var doc = bsonValue.AsBsonDocument;
            return new GradientAlphaKey(
                (float)doc["Alpha"].ToDouble(),
                (float)doc["Time"].ToDouble()
            );
        }).ToArray();

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }

    // 序列化
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Gradient value)
    {
        if (value == null || context == null)
        {
            BsonSerializer.Serialize(context.Writer, new BsonDocument());
            return;
        }
            
        var document = new BsonDocument
        {
            {
                "ColorKeys", new BsonArray(value.colorKeys.Select(key => new BsonDocument
                {
                    { "R", key.color.r },
                    { "G", key.color.g },
                    { "B", key.color.b },
                    { "A", key.color.a },
                    { "Time", key.time }
                }))
            },
            {
                "AlphaKeys", new BsonArray(value.alphaKeys.Select(key => new BsonDocument
                {
                    { "Alpha", key.alpha },
                    { "Time", key.time }
                }))
            }
        };

        BsonSerializer.Serialize(context.Writer, document);
    }

    public Type ValueType => typeof(Gradient);
}
