using System;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

namespace PCITC.MES.MM.Tapper.Framework.Serializing
{
    public class DefaultBinarySerializer : IBinarySerializer
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// 对象二进制序列化为byte数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                _binaryFormatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化弱类型对象对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(byte[] data, Type type)
        {
            using (var stream = new MemoryStream(data))
            {
                return _binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 从二进制数字反序列化指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] data) where T : class
        {
            using (var stream = new MemoryStream(data))
            {
                return _binaryFormatter.Deserialize(stream) as T;
            }
        }
    }
}
