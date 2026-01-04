//------------------------------------------------------------
// File: DeepCopyHelper.cs
// Created: 2025-12-05
// Purpose: Extension methods for deep-copying objects using BSON serialization.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public static class DeepCloneHelper
    {
        [ThreadStatic] private static MemoryStream _cachedStream;
        private const int DefaultStreamSize = 64 * 1024;

        /// <summary>
        /// Deep copy via BSON serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T obj)
            where T : class
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    BsonSerializer.Serialize(new BsonBinaryWriter(stream), obj);
                    stream.Position = 0;
                    return  BsonSerializer.Deserialize<T>(stream);
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Deep copy via a cached MemoryStream to reduce allocations (thread-safe via lock).
        /// </summary>
        public static T DeepCopyFast<T>(this T obj) where T : class
        {
            if (obj == null) return null;

            var stream = _cachedStream ?? (_cachedStream = new MemoryStream(DefaultStreamSize));
            lock (stream)
            {
                try
                {
                    stream.Position = 0;
                    stream.SetLength(0);
                    BsonSerializer.Serialize(new BsonBinaryWriter(stream), obj);
                    stream.Position = 0;
                    return BsonSerializer.Deserialize<T>(stream);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return null;
                }
            }
        }
        
        public static T DeepCopyByReflect<T>(T obj)
        {
            // Return immediately for strings or value types.
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}
