using System.Collections;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 字典写入流中
    /// </summary>
    public class IDictionaryProvider
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static IDictionaryProvider Default { get; } = new IDictionaryProvider();

        #endregion ctor

        #region write

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IDictionary array)
        {
            if (array == null)
            {
                if (setting.WriteNullWhenObjectIsNull)
                {
                    writer.Write("null");
                }
                else
                {
                    writer.Write("{}");
                }

                return;
            }

            if (array.Count == 0)
            {
                writer.Write("{}");
                return;
            }

            writer.Write("{");

            var enumerator = array.Keys.GetEnumerator();
            bool moveNext = enumerator.MoveNext();
            while (moveNext)
            {
                SerialierBuilderHelper.QueryBuilderInvoker(enumerator.Current.GetType())(writer, setting, enumerator.Current, level++);
                writer.Write(":");
                var item = array[enumerator.Current];
                SerialierBuilderHelper.QueryBuilderInvoker(item.GetType())(writer, setting, item, level++);
                if (moveNext = enumerator.MoveNext())
                    writer.Write(",");
            }

            writer.Write("}");
            return;
        }

        #endregion write
    }
}