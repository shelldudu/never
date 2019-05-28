using Never.Serialization.Json.MethodProviders;
using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 枚举类型的数组写入流
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullableEnumEnumerableProvider<T> where T : struct, IConvertible
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumEnumerableProvider{T}"/> class.
        /// </summary>
        public NullableEnumEnumerableProvider()
        {
            this.MethodProvider = EnumMethodProvider<T>.Default;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static NullableEnumEnumerableProvider<T> Default { get; } = new NullableEnumEnumerableProvider<T>();

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IEnumerable<Nullable<T>> array)
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

            var list = array as IList<Nullable<T>>;
            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].HasValue)
                    {
                        this.MethodProvider.Write(writer, setting, list[i].Value);
                    }
                    else
                    {
                        writer.Write("null");
                    }

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
                if (enumerator.Current.HasValue)
                {
                    this.MethodProvider.Write(writer, setting, enumerator.Current.Value);
                }
                else
                {
                    writer.Write("null");
                }
            }
            while (enumerator.MoveNext())
            {
                writer.Write(",");
                if (enumerator.Current.HasValue)
                {
                    this.MethodProvider.Write(writer, setting, enumerator.Current.Value);
                }
                else
                {
                    writer.Write("null");
                }
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