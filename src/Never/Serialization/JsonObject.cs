using Never.Serialization.Json;
using Never.Serialization.Json.Deserialize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Serialization
{
    /// <summary>
    /// json节点对象
    /// </summary>
    public sealed class JsonObject
    {
        #region field and ctor
        private readonly string json;
        private readonly IDeserializerReader reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public JsonObject(string json)
        {
            this.json = json;
            this.reader = new ThunderReader(json);
        }

        /// <summary>
        /// 
        /// </summary>
        public JsonObject(object target) : this(EasyJsonSerializer.SerializeObject(target))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        internal JsonObject(IObjectContentNode node)
        {
            this.json = node.ToString();
            this.reader = ThunderReader.Load(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal JsonObject(IDeserializerReader reader)
        {
            this.reader = reader;
            this.json = reader.ToString();
        }

        #endregion

        /// <summary>
        /// 转换成一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Parse<T>()
        {
            return EasyJsonSerializer.Deserialize<T>(this.reader);
        }

        /// <summary>
        /// 转换成一个对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Parse(Type type)
        {
            return EasyJsonSerializer.DeserializeObject(this.reader, type);
        }

        /// <summary>
        /// 包含了key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this.ContainsKey(key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 包含了key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public bool ContainsKey(string key, StringComparison comparison)
        {
            if (key.IsNullOrEmpty())
                return false;

            return this.reader.Read(key, comparison) != null;
        }

        /// <summary>
        /// 尝试获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetString(string key, out string value)
        {
            return this.TryGetString(key, StringComparison.OrdinalIgnoreCase, out value);
        }

        /// <summary>
        /// 尝试获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="comparison"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetString(string key, StringComparison comparison, out string value)
        {
            value = string.Empty;
            if (key.IsNullOrEmpty())
                return false;

            var node = this.reader.Read(key, comparison);
            if (node == null)
                return false;

            value = node.ToString();
            return true;
        }

        /// <summary>
        /// 尝试获取并序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            return this.TryGetValue<T>(key, StringComparison.OrdinalIgnoreCase, out value);
        }

        /// <summary>
        /// 尝试获取并序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="comparison"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string key, StringComparison comparison, out T value)
        {
            value = default(T);
            if (key.IsNullOrEmpty())
                return false;

            var node = this.reader.Read(key, comparison);
            if (node == null)
                return false;

            value = EasyJsonSerializer.Deserialize<T>(ThunderReader.Load(node));
            return true;
        }

        /// <summary>
        /// 返回一个json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return this.json;
        }
    }
}
