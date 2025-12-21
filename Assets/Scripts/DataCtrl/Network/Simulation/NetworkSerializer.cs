using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// Using binary formatter for simulation simplicity as BSON/JSON might require more setup for structs.
// In a real project, we might use MemoryPack or Protobuf.

namespace Ebonor.DataCtrl
{
    public static class NetworkSerializer
    {
        public static byte[] Serialize<T>(T data)
        {
            if (data == null) return null;
            
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
