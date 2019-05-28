using Never.Serialization.Json.Deserialize;
using Never.Serialization.Json.Serialize;
using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// 枚举与流内容操作的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumMethodProvider<T> : ConvertMethodProvider<T>, IConvertMethodProvider<T> where T : struct, IConvertible
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static EnumMethodProvider<T> Default { get; } = new EnumMethodProvider<T>();

        #endregion ctor

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="EnumMethodProvider{T}"/> class.
        /// </summary>
        public EnumMethodProvider()
        {
        }

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, T source)
        {
            writer.Write(EnumSerializerBuilder<T>.Register(setting).Invoke(source));
        }

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="node">节点流内容</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override T Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return default(T);

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return default(T);

            return EnumDeseralizerBuilder<T>.Register(setting).Invoke(value.ToString());
        }

        #endregion IMethodProvider
    }
}