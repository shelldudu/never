using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 基元类型的数组写入流
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimitiveEnumerableProvider<T>
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveEnumerableProvider{T}"/> class.
        /// </summary>
        /// <param name="methodProvider">The method provider.</param>
        public PrimitiveEnumerableProvider(IConvertMethodProvider<T> methodProvider)
        {
            this.MethodProvider = methodProvider;
        }

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<T> array)
        {
            if (!setting.WriteQuoteWhenObjectIsNumber)
            {
                this.WriteString(writer, setting, array);
                return;
            }

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

            var provider = this.MethodProvider;
            var list = array as IList<T>;
            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    writer.Write("\"");
                    provider.Write(writer, setting, list[i]);
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
                provider.Write(writer, setting, enumerator.Current);
                writer.Write("\"");
            }

            while (enumerator.MoveNext())
            {
                writer.Write(",");
                writer.Write("\"");
                provider.Write(writer, setting, enumerator.Current);
                writer.Write("\"");
            }

            writer.Write("]");
            return;
        }

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="array">The array.</param>
        public virtual void WriteString(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<T> array)
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

            var provider = this.MethodProvider;
            var list = array as IList<T>;
            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    provider.Write(writer, setting, list[i]);
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
                provider.Write(writer, setting, enumerator.Current);
            }

            while (enumerator.MoveNext())
            {
                writer.Write(",");
                provider.Write(writer, setting, enumerator.Current);
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