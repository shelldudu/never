using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 字典写入流中
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class IDictionaryProvider<Key, Value>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static IDictionaryProvider<Key, Value> Default { get; } = new IDictionaryProvider<Key, Value>();

        #endregion ctor

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static IDictionaryProvider()
        {
        }

        #endregion ctor

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IDictionary<Key, Value> array)
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
            var i = -1;
            foreach (var a in array)
            {
                i++;
                ZzzZzSerialierBuilder<Key>.Register(setting).Invoke(writer, setting, a.Key, level++);
                writer.Write(":");
                ZzzZzSerialierBuilder<Value>.Register(setting).Invoke(writer, setting, a.Value, level++);
                if (i < array.Count - 1)
                    writer.Write(",");
            }

            writer.Write("}");
            return;
        }

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="array">The array.</param>
        public virtual void WriteObjectValue(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IDictionary<Key, object> array)
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
            var i = -1;
            foreach (var a in array)
            {
                i++;
                ZzzZzSerialierBuilder<Key>.Register(setting).Invoke(writer, setting, a.Key, level++);
                writer.Write(":");

                if (a.Value == null)
                {
                    if (setting.WriteNullWhenObjectIsNull)
                    {
                        writer.Write("null");
                    }
                    else
                    {
                        writer.Write("{}");
                    }
                }
                else
                {
                    SerialierBuilderHelper.QueryBuilderInvoker(a.Value.GetType()).Invoke(writer, setting, a.Value, level++);
                }

                if (i < array.Count - 1)
                    writer.Write(",");
            }

            writer.Write("}");
            return;
        }
    }
}