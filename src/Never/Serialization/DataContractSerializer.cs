using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Never.Serialization
{
    /// <summary>
    /// Runtime.Json 对象序化列
    /// </summary>
    public struct DataContractSerializer : IJsonSerializer
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractSerializer"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        private DataContractSerializer(Encoding encoding)
        {
            this.encoding = encoding ?? Encoding.UTF8;
        }

        #endregion ctor

        #region encoding

        /// <summary>
        /// 数据编码
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// 数据编码
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this.encoding ?? Encoding.UTF8;
            }
        }

        #endregion

        #region 序列化

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public string SerializeObject(object @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            var ser = new DataContractJsonSerializer(@object.GetType());
            using (var st = new MemoryStream())
            {
                ser.WriteObject(st, @object);
                return this.Encoding.GetString(st.ToArray());
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public string Serialize<T>(T @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            var ser = new DataContractJsonSerializer(@object.GetType());
            using (var st = new MemoryStream())
            {
                ser.WriteObject(st, @object);
                return this.Encoding.GetString(st.ToArray());
            }
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public T Deserialize<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(T);
            }

            return (T)this.DeserializeObject(input, typeof(T));
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object DeserializeObject(string input, Type targetType)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var bytes = this.Encoding.GetBytes(input);
            var ser = new DataContractJsonSerializer(targetType);
            using (var st = new MemoryStream(bytes))
            {
                return ser.ReadObject(st);
            }
        }

        #endregion 反序列化
    }
}