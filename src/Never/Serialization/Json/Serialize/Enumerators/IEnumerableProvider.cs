using System.Collections;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 数组写入流中
    /// </summary>
    public class IEnumerableProvider
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static IEnumerableProvider Default { get; } = new IEnumerableProvider();

        #endregion ctor

        #region write

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IEnumerable array)
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

            writer.Write("[");
            var enumerator = array.GetEnumerator();
            bool moveNext = enumerator.MoveNext();
            while (moveNext)
            {
                var item = enumerator.Current;
                SerialierBuilderHelper.QueryBuilderInvoker(item.GetType())(writer, setting, item, level++);
                if (moveNext = enumerator.MoveNext())
                    writer.Write(",");
            }
            writer.Write("]");
            return;
        }

        #endregion write
    }
}