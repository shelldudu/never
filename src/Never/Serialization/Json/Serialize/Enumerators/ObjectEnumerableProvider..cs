using System.Collections;
using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize.Enumerators
{
    /// <summary>
    /// 数组写入流中
    /// </summary>
    public class ObjectEnumerableProvider
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static ObjectEnumerableProvider Default { get; } = new ObjectEnumerableProvider();

        #endregion ctor

        #region write

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="array">The array.</param>
        /// <param name="level"></param>
        public virtual void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte level, IEnumerable<object> array)
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

            var list = array as IList<object>;

            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    var @str = item as string;
                    if (str != null)
                    {
                        ZzzZzSerialierBuilder<string>.Register(setting).Invoke(writer, setting, @str, level++);
                        if (i < list.Count - 1)
                            writer.Write(",");

                        continue;
                    }

                    var inlineArray = item as IEnumerable;
                    if (inlineArray != null)
                    {
                        writer.Write("[");
                        var listEnumerator = inlineArray.GetEnumerator();
                        if (listEnumerator.MoveNext())
                        {
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        while (listEnumerator.MoveNext())
                        {
                            writer.Write(",");
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        writer.Write("]");
                        if (i < list.Count - 1)
                            writer.Write(",");
                        continue;
                    }

                    if (item != null)
                    {
                        SerialierBuilderHelper.QueryBuilderInvoker(item.GetType())(writer, setting, item, level++);
                    }
                    else
                    {
                        writer.Write("null");
                    }

                    if (i < list.Count - 1)
                        writer.Write(",");

                    continue;
                }
                writer.Write("]");
                return;
            }

            writer.Write("[");
            var enumerator = array.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var @str = enumerator.Current as string;
                if (str != null)
                {
                    ZzzZzSerialierBuilder<string>.Register(setting).Invoke(writer, setting, @str, level++);
                }
                else
                {
                    var inlineArray = enumerator.Current as IEnumerable;
                    if (inlineArray != null)
                    {
                        writer.Write("[");
                        var listEnumerator = inlineArray.GetEnumerator();
                        if (listEnumerator.MoveNext())
                        {
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        while (listEnumerator.MoveNext())
                        {
                            writer.Write(",");
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }
                        writer.Write("]");
                    }
                    else
                    {
                        if (enumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(enumerator.Current.GetType())(writer, setting, enumerator.Current, level++);
                        }
                    }
                }
            }

            while (enumerator.MoveNext())
            {
                writer.Write(",");
                var @str = enumerator.Current as string;
                if (str != null)
                {
                    ZzzZzSerialierBuilder<string>.Register(setting).Invoke(writer, setting, @str, level++);
                }
                else
                {
                    var inlineArray = enumerator.Current as IEnumerable;
                    if (inlineArray != null)
                    {
                        writer.Write("[");
                        var listEnumerator = inlineArray.GetEnumerator();
                        if (listEnumerator.MoveNext())
                        {
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        while (listEnumerator.MoveNext())
                        {
                            writer.Write(",");
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }
                        writer.Write("]");
                    }
                    else
                    {
                        if (enumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(enumerator.Current.GetType())(writer, setting, enumerator.Current, level++);
                        }
                    }
                }
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
        /// <param name="level"></param>
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

            var list = array as IList<object>;

            if (list != null)
            {
                writer.Write("[");
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    var inlineArray = item as IEnumerable;
                    if (inlineArray != null)
                    {
                        writer.Write("[");
                        var listEnumerator = inlineArray.GetEnumerator();
                        if (listEnumerator.MoveNext())
                        {
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        while (listEnumerator.MoveNext())
                        {
                            writer.Write(",");
                            if (listEnumerator.Current == null)
                            {
                                writer.Write("null");
                            }
                            else
                            {
                                SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                            }
                        }

                        writer.Write("]");
                        if (i < list.Count - 1)
                            writer.Write(",");
                        continue;
                    }

                    if (item != null)
                    {
                        SerialierBuilderHelper.QueryBuilderInvoker(item.GetType())(writer, setting, item, level++);
                    }
                    else
                    {
                        writer.Write("null");
                    }

                    if (i < list.Count - 1)
                        writer.Write(",");

                    continue;
                }
                writer.Write("]");
                return;
            }

            writer.Write("[");
            var enumerator = array.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var inlineArray = enumerator.Current as IEnumerable;
                if (inlineArray != null)
                {
                    writer.Write("[");
                    var listEnumerator = inlineArray.GetEnumerator();
                    if (listEnumerator.MoveNext())
                    {
                        if (listEnumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                        }
                    }

                    while (listEnumerator.MoveNext())
                    {
                        writer.Write(",");
                        if (listEnumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                        }
                    }
                    writer.Write("]");
                }
                else
                {
                    if (enumerator.Current == null)
                    {
                        writer.Write("null");
                    }
                    else
                    {
                        SerialierBuilderHelper.QueryBuilderInvoker(enumerator.Current.GetType())(writer, setting, enumerator.Current, level++);
                    }
                }
            }

            while (enumerator.MoveNext())
            {
                writer.Write(",");
                var inlineArray = enumerator.Current as IEnumerable;
                if (inlineArray != null)
                {
                    writer.Write("[");
                    var listEnumerator = inlineArray.GetEnumerator();
                    if (listEnumerator.MoveNext())
                    {
                        if (listEnumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                        }
                    }

                    while (listEnumerator.MoveNext())
                    {
                        writer.Write(",");
                        if (listEnumerator.Current == null)
                        {
                            writer.Write("null");
                        }
                        else
                        {
                            SerialierBuilderHelper.QueryBuilderInvoker(listEnumerator.Current.GetType())(writer, setting, listEnumerator.Current, level++);
                        }
                    }
                    writer.Write("]");
                }
                else
                {
                    if (enumerator.Current == null)
                    {
                        writer.Write("null");
                    }
                    else
                    {
                        SerialierBuilderHelper.QueryBuilderInvoker(enumerator.Current.GetType())(writer, setting, enumerator.Current, level++);
                    }
                }
            }

            writer.Write("]");
            return;
        }

        #endregion write
    }
}