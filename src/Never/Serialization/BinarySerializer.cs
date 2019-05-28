using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Never.Serialization
{
    /// <summary>
    /// 2进制的序化列对象
    /// </summary>
    public struct BinarySerializer : IBinarySerializer
    {
        #region field

        /// <summary>
        /// 对象格式化
        /// </summary>
        private readonly static IFormatter format = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="BinarySerializer"/> class.
        /// </summary>
        static BinarySerializer()
        {
            format = new BinaryFormatter();
        }

        #endregion ctor

        #region 序列化

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        public byte[] SerializeObject(object @object)
        {
            if (@object == null)
                return null;

            using (var st = new MemoryStream())
            {
                this.Serialize(st, @object);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        public void Serialize(Stream stream, object @object)
        {
            if (@object == null)
                return;

            format.Serialize(stream, @object);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        public byte[] Serialize<T>(T @object)
        {
            if (@object == null)
                return null;

            using (var st = new MemoryStream())
            {
                this.Serialize(st, @object);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        public void Serialize<T>(Stream stream, T @object)
        {
            format.Serialize(stream, @object);
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="bytes">源数据</param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
                return default(T);

            using (var st = new MemoryStream(bytes))
            {
                return (T)format.Deserialize(st);
            };
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                return default(T);

            return (T)format.Deserialize(stream);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="bytes">源数据</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object DeserializeObject(byte[] bytes, Type targetType)
        {
            if (bytes == null)
                return null;

            using (var st = new MemoryStream(bytes))
            {
                return format.Deserialize(st);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public object Deserialize(Stream stream)
        {
            if (stream == null)
                return null;

            return format.Deserialize(stream);
        }

        #endregion 反序列化
    }
}