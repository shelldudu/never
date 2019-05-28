using Never.Reflection;
using Never.Serialization.Json.Serialize;
using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.MethodProviders.Nullables
{
    /// <summary>
    /// 枚举与流内容操作的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumMethodProvider<T> : ConvertMethodProvider<Nullable<T>>, IConvertMethodProvider<Nullable<T>> where T : struct, IConvertible
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
        static EnumMethodProvider()
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
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, Nullable<T> source)
        {
            if (source.HasValue)
            {
                if (setting.WriteNumberOnEnumType)
                {
                    if (setting.WriteQuoteWhenObjectIsNumber)
                    {
                        writer.Write("\"");
                        MethodProviders.EnumMethodProvider<T>.Default.Write(writer, setting, source.Value);
                        writer.Write("\"");
                    }
                    else
                    {
                        MethodProviders.EnumMethodProvider<T>.Default.Write(writer, setting, source.Value);
                    }
                }
                else
                {
                    writer.Write("\"");
                    MethodProviders.EnumMethodProvider<T>.Default.Write(writer, setting, source.Value);
                    writer.Write("\"");
                }

                return;
            }

            writer.Write("null");
        }

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="node">节点流内容</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override Nullable<T> Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return null;

            if (checkNullValue && this.IsNullValue(value))
                return null;

            return MethodProviders.EnumMethodProvider<T>.Default.Parse(reader, setting, node, checkNullValue);
        }

        #endregion IMethodProvider
    }
}