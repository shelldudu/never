using Never.Serialization.Json.MethodProviders;
using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 枚举类型的数组写入流
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumEnumerableProvider<T> where T : struct, IConvertible
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumEnumerableProvider{T}"/> class.
        /// </summary>
        public EnumEnumerableProvider()
        {
            this.MethodProvider = EnumMethodProvider<T>.Default;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static EnumEnumerableProvider<T> Default { get; } = new EnumEnumerableProvider<T>();

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IEnumerable<T> array)
        {
            if (array == null)
            {
                if (setting.WriteNullWhenObjectIsNull)
                {
                    writer.Write("null");
                }
                else
                {
                    writer.Write("[]");
                }

                return;
            }

            if (setting.WriteNumberOnEnumType)
            {
                if (setting.WriteQuoteWhenObjectIsNumber)
                {
                    this.WriteQuote(writer, setting, level, array);
                    return;
                }
                else
                {
                    var list = array as IList<T>;
                    if (list != null)
                    {
                        writer.Write("[");
                        for (var i = 0; i < list.Count; i++)
                        {
                            this.MethodProvider.Write(writer, setting, list[i]);
                            if (i < list.Count - 1)
                                writer.Write(",");
                        }
                        writer.Write("]");
                        return;
                    }

                    writer.Write("[");
                    var enumerator = array.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        this.MethodProvider.Write(writer, setting, enumerator.Current);
                    }
                    while (enumerator.MoveNext())
                    {
                        writer.Write(",");
                        this.MethodProvider.Write(writer, setting, enumerator.Current);
                    }

                    writer.Write("]");
                    return;
                }
            }
            else
            {
                this.WriteQuote(writer, setting, level, array);
                return;
            }
        }

        private void WriteQuote(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IEnumerable<T> array)
        {
            var list = array as IList<T>;
            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    writer.Write("\"");
                    this.MethodProvider.Write(writer, setting, list[i]);
                    writer.Write("\"");
                    if (i < list.Count - 1)
                        writer.Write(",");
                }
                writer.Write("]");
                return;
            }

            writer.Write("[");
            var enumerator = array.GetEnumerator();
            if (enumerator.MoveNext())
            {
                writer.Write("\"");
                this.MethodProvider.Write(writer, setting, enumerator.Current);
                writer.Write("\"");
            }
            while (enumerator.MoveNext())
            {
                writer.Write(",");
                writer.Write("\"");
                this.MethodProvider.Write(writer, setting, enumerator.Current);
                writer.Write("\"");
            }

            writer.Write("]");
            return;
        }

        /// <summary>
        /// 获取方法转换
        /// </summary>
        /// <returns>IMethodProvider&lt;T&gt;.</returns>
        public IConvertMethodProvider<T> MethodProvider { get; protected set; }

        #endregion IMethodProvider
    }
}