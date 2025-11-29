using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Linq;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class AnimationCurveSerializer : IBsonSerializer<AnimationCurve>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (AnimationCurve)value);
        }

        public Type ValueType => typeof(AnimationCurve);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AnimationCurve value)
        {
            if (null == value)
            {
                value = new AnimationCurve();
            }
            
            if (value.keys.Length == 0)
            {
                value.AddKey(new Keyframe(0, 0));
            }
            
            var document = new BsonDocument
            {
                { "Keys", new BsonArray(value.keys.Select(key => new BsonDocument
                {
                    { "Time", key.time },
                    { "Value", key.value },
                    { "InTangent", key.inTangent },
                    { "OutTangent", key.outTangent },
                    { "TangentMode", (int)key.weightedMode }
                })) },
                { "PreWrapMode", (int)value.preWrapMode },
                { "PostWrapMode", (int)value.postWrapMode }
            };
            BsonSerializer.Serialize(context.Writer, typeof(BsonDocument), document);
           

           
        }

        public AnimationCurve Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);

            if (document.Contains("Keys"))
            {
                var keys = document["Keys"].AsBsonArray
                    .Select(bsonValue =>
                    {
                        var doc = bsonValue.AsBsonDocument;
                        return new Keyframe(
                            (float)doc["Time"].ToDouble(),
                            (float)doc["Value"].ToDouble(),
                            (float)doc["InTangent"].ToDouble(),
                            (float)doc["OutTangent"].ToDouble()
                        )
                        {
                            weightedMode = doc.Contains("WeightedMode") 
                                ? (WeightedMode)doc["WeightedMode"].ToInt32() 
                                : WeightedMode.None // default value to handle missing fields
                        };
                    })
                    .ToArray();
                return new AnimationCurve(keys)
                {
                    preWrapMode = (WrapMode)document["PreWrapMode"].AsInt32,
                    postWrapMode = (WrapMode)document["PostWrapMode"].AsInt32
                };
            }
            
            return new AnimationCurve()
            {
                preWrapMode = (WrapMode)document["PreWrapMode"].AsInt32,
                postWrapMode = (WrapMode)document["PostWrapMode"].AsInt32
            };

           
        }
    }
}
