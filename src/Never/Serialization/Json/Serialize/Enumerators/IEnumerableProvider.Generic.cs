using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 数组写入流中
    /// </summary>
    public class IEnumerableProvider<T>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static IEnumerableProvider<T> Default { get; } = new IEnumerableProvider<T>();

        #endregion ctor

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static IEnumerableProvider()
        {
        }

        #endregion ctor

        #region write

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

            var list = array as IList<T>;
            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    ZzzZzSerialierBuilder<T>.Register(setting).Invoke(writer, setting, list[i], level++);
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
                ZzzZzSerialierBuilder<T>.Register(setting).Invoke(writer, setting, enumerator.Current, level++);
            }
            while (enumerator.MoveNext())
            {
                writer.Write(",");
                ZzzZzSerialierBuilder<T>.Register(setting).Invoke(writer, setting, enumerator.Current, level++);
            }
            writer.Write("]");
            return;
        }

        #endregion write
    }
}