using System;
using System.Collections;
using System.Collections.Generic;

namespace Never.Serialization.Json.Deserialize.Enumerators
{
    /// <summary>
    /// 字典对象类型的数组写入流
    /// </summary>
    public class IDictionaryProvider
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="IDictionaryProvider"/> class.
        /// </summary>
        public IDictionaryProvider()
        {
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static IDictionaryProvider Default { get; } = new IDictionaryProvider();

        #endregion ctor

        #region load

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="reader">The array.</param>
        /// <param name="arrayLevel"></param>
        public virtual void Load(IDictionary dictionary, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var node = reader.Read(name);
            if (node == null)
            {
                if (name != null)
                    return;

                while (true)
                {
                    var item = reader.MoveNext();
                    if (item == null)
                        break;

                    dictionary[item.Key] = item.ToString();
                    //if (item.NodeType == ContentNodeType.String)
                    //{
                    //    dictionary[item.Key] = item.ToString();
                    //}
                    //else
                    //{
                    //    dictionary[item.Key] = item.ToString();
                    //    //dictionary[item.Key] = reader.Read(item.StartIndex - 1, item.EndIndex + 1);
                    //}
                }

                return;
            }

            if (node.NodeType == ContentNodeType.String)
                return;

            if (node.NodeType != ContentNodeType.Object)
                throw new ArgumentException(string.Format("字典只能为key-value形式，当前形式为{0}", node.NodeType.ToString()));

            foreach (var item in (IList<JsonContentNode>)node.Node)
            {
                dictionary[item.Key] = item.ToString();
                //if (item.NodeType == ContentNodeType.String)
                //{
                //    dictionary[item.Key] = new string(item.GetValue());
                //}
                //else
                //{
                //    dictionary[item.Key] = item.ToString();
                //    //dictionary[n.Key] = reader.Read(n.StartIndex - 1, n.EndIndex + 1);
                //}
            }
        }

        #endregion load

        #region generic load

        /// <summary>
        /// 装载数据到字典中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The result.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="name">The name.</param>
        /// <param name="arrayLevel"></param>
        public virtual void Load<T, V>(IDictionary<T, V> dictionary, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var node = reader.Read(name);
            if (node == null)
            {
                if (name != null)
                    return;

                while (true)
                {
                    var item = reader.MoveNext();
                    if (item == null)
                        break;

                    var key = ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(ThunderReader.Load(item.Key), setting, arrayLevel);
                    var value = ZzzZzDeserializerBuilder<V>.Register(setting).Invoke(reader.Parse(item), setting, arrayLevel);
                    dictionary[key] = value;
                }

                return;
            }

            if (node.NodeType == ContentNodeType.String)
                return;

            if (node.NodeType != ContentNodeType.Object)
                throw new ArgumentException(string.Format("字典只能为key-value形式，当前形式为{0}", node.NodeType.ToString()));

            foreach (var n in (IList<JsonContentNode>)node.Node)
            {
                var key = ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(ThunderReader.Load(n.Key), setting, arrayLevel);
                var strkey = key as string;
                var value = ZzzZzDeserializerBuilder<V>.Register(setting).Invoke(reader.Parse(n), setting, arrayLevel);
                dictionary[key] = value;
            }
        }

        /// <summary>
        /// 装载数据到字典中
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The result.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="name">The name.</param>
        /// <param name="arrayLevel"></param>
        public virtual void LoadStringKey<V>(IDictionary<string, V> dictionary, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var node = reader.Read(name);
            if (node == null)
            {
                if (name != null)
                    return;

                while (true)
                {
                    var item = reader.MoveNext();
                    if (item == null)
                        break;

                    var key = ZzzZzDeserializerBuilder<string>.Register(setting).Invoke(ThunderReader.Load(item.Key), setting, arrayLevel);
                    if (string.IsNullOrEmpty(key))
                        continue;

                    var value = ZzzZzDeserializerBuilder<V>.Register(setting).Invoke(reader.Parse(item), setting, arrayLevel);
                    dictionary[key] = value;
                }

                return;
            }

            if (node.NodeType == ContentNodeType.String)
                return;

            if (node.NodeType != ContentNodeType.Object)
                throw new ArgumentException(string.Format("字典只能为key-value形式，当前形式为{0}", node.NodeType.ToString()));

            foreach (var n in (IList<JsonContentNode>)node.Node)
            {
                var key = ZzzZzDeserializerBuilder<string>.Register(setting).Invoke(ThunderReader.Load(n.Key), setting, arrayLevel);
                if (string.IsNullOrEmpty(key))
                    continue;

                var value = ZzzZzDeserializerBuilder<V>.Register(setting).Invoke(reader.Parse(n), setting, arrayLevel);
                dictionary[key] = value;
            }
        }

        /// <summary>
        /// 装载数据到字典中
        /// </summary>
        /// <param name="dictionary">The result.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="name">The name.</param>
        /// <param name="arrayLevel"></param>
        public virtual void LoadStringKeyStringvalue(IDictionary<string, string> dictionary, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var node = reader.Read(name);
            if (node == null)
            {
                if (name != null)
                    return;

                while (true)
                {
                    var item = reader.MoveNext();
                    if (item == null)
                        break;

                    var key = ZzzZzDeserializerBuilder<string>.Register(setting).Invoke(ThunderReader.Load(item.Key), setting, arrayLevel);
                    if (string.IsNullOrEmpty(key))
                        continue;

                    var itemValue = item.GetValue();
                    if (itemValue == null)
                    {
                        dictionary[key] = "";
                        continue;
                    }

                    dictionary[item.Key] = item.ToString();
                    //if (item.NodeType == ContentNodeType.String)
                    //{
                    //    dictionary[key] = new string(itemValue);
                    //}
                    //else
                    //{
                    //    dictionary[item.Key] = item.ToString();
                    //}
                }

                return;
            }

            if (node.NodeType == ContentNodeType.String)
                return;

            if (node.NodeType != ContentNodeType.Object)
                throw new ArgumentException(string.Format("字典只能为key-value形式，当前形式为{0}", node.NodeType.ToString()));

            foreach (var item in (IList<JsonContentNode>)node.Node)
            {
                var key = ZzzZzDeserializerBuilder<string>.Register(setting).Invoke(ThunderReader.Load(item.Key), setting, arrayLevel);
                if (string.IsNullOrEmpty(key))
                    continue;

                var nValue = item == null ? ArraySegmentValue.Empty : item.GetValue();
                if (nValue.IsNullOrEmpty)
                {
                    dictionary[key] = "";
                    continue;
                }

                dictionary[item.Key] = item.ToString(); 
                //if (item.NodeType == ContentNodeType.String)
                //{
                //    dictionary[key] = new string(nValue);
                //}
                //else
                //{
                //    dictionary[item.Key] = item.ToString();
                //}
            }
        }

        #endregion generic load
    }
}