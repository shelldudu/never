using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.Serialization
{
    /// <summary>
    /// xml对象序化列
    /// </summary>
    public struct XmlSerializer : IXmlSerializer
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public XmlSerializer(Encoding encoding)
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

            var bytes = this.SerializeBytes(@object);
            return this.Encoding.GetString(bytes);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public string Serialize<T>(T @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            var bytes = this.SerializeBytes(@object);
            return this.Encoding.GetString(bytes);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        private byte[] SerializeBytes(object @object)
        {
            if (@object == null)
            {
                return null;
            }

            var ser = new System.Xml.Serialization.XmlSerializer(@object.GetType());
            using (var ms = new MemoryStream() { Position = 0 })
            {
                ser.Serialize(ms, @object);
                return ms.ToArray();
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
            return this.Deserialize(bytes, targetType);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="bytes">源数据</param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(bytes) { Position = 0 })
            {
                return (T)ser.Deserialize(ms);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="bytes">源数据</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object Deserialize(byte[] bytes, Type targetType)
        {
            if (bytes == null)
            {
                return null;
            }

            var ser = new System.Xml.Serialization.XmlSerializer(targetType);
            using (var ms = new MemoryStream(bytes) { Position = 0 })
            {
                return ser.Deserialize(ms);
            }
        }

        #endregion 反序列化

        #region 替换xml的特殊字符

        /// <summary>
        /// 还原xml内容特殊字符
        /// </summary>
        /// <param name="text">xml内容</param>
        /// <returns></returns>
        public static string XmlDecode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(text);
            return sb.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();
        }

        /// <summary>
        /// 替换xml内容特殊字符
        /// </summary>
        /// <param name="text">xml内容</param>
        /// <returns></returns>
        public static string XmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(text);
            return sb.Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").ToString();
        }

        /// <summary>
        /// 替换xml内容非法字符
        /// </summary>
        /// <param name="text">xml内容</param>
        /// <returns></returns>
        public static string XmlReplaceIllegalWord(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return Regex.Replace(text, "[\\x00-\\x08\\x0b-\\x0c\\x0e-\\x1f]", string.Empty, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        #endregion 替换xml的特殊字符

        #region load

        /// <summary>
        /// 从字符串中返回一个DataSet
        /// </summary>
        /// <param name="xml">Xml格式内容</param>
        /// <returns></returns>
        public static DataSet Load(string xml)
        {
            return Load(xml, Encoding.UTF8);
        }

        /// <summary>
        /// 从字符串中返回一个DataSet
        /// </summary>
        /// <param name="xml">Xml格式内容</param>
        /// <param name="encoding">Xml格式内容字符编码</param>
        /// <returns></returns>
        public static DataSet Load(string xml, Encoding encoding)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var ds = new DataSet();
            using (var st = new MemoryStream(encoding.GetBytes(xml)) { Position = 0 })
            {
                ds.ReadXmlSchema(st);
                foreach (DataTable dt in ds.Tables)
                {
                    dt.BeginLoadData();
                }

                ds.ReadXml(st, XmlReadMode.IgnoreSchema);

                foreach (DataTable dt in ds.Tables)
                {
                    dt.EndLoadData();
                }
            }

            return ds;
        }

        #endregion load
    }
}