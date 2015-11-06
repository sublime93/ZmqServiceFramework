using System;
using MsgPack.Serialization;

namespace Common
{
    public class Serializer : ISerializer
    {
        public byte[] Serialize<T>(T data)
        {
            var ser = MessagePackSerializer.Get<T>();
            return ser.PackSingleObject(data);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return (T)Deserialize(typeof (T), bytes);
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            var deser = MessagePackSerializer.Get(type);
            return deser.UnpackSingleObject(bytes);
        }
    }
}
