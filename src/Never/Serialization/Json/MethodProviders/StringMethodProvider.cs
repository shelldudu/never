using Never.Serialization.Json.Deserialize;
using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// string 值转换
    /// </summary>
    public sealed class StringMethodProvider : ConvertMethodProvider<string>, IConvertMethodProvider<string>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static StringMethodProvider Default { get; } = new StringMethodProvider();

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="node">节点流内容</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override string Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var objnode = node as IObjectContentNode;
            if (objnode == null)
                return null;

            var objnodeValue = node.GetValue();
            if (objnode.NodeType == ContentNodeType.String)
            {
                if (objnodeValue == null)
                    return null;

                if (objnodeValue.Length == 0)
                    return string.Empty;

                if (checkNullValue && this.IsNullValue(objnodeValue))
                    return null;

                if (!objnode.Escaping)
                    return objnodeValue.ToString();

                /*有转义符号*/
                var buffer = new char[objnodeValue.Length];
                int index = 0;
                for (var i = 0; i < objnodeValue.Length; i++)
                {
                    if (i >= objnodeValue.Length)
                        continue;

                    switch (objnodeValue[i])
                    {
                        case '\\':
                            {
                                var pre = this.Next(objnodeValue, i);
                                if (!pre.HasValue)
                                    continue;

                                switch (pre.Value)
                                {
                                    case '/':
                                    case '\'':
                                    case '\"':
                                        {
                                            i += 1;
                                            buffer[index] = objnodeValue[i];
                                            index++;
                                        }
                                        break;

                                    case 'b':
                                        {
                                            buffer[index] = '\b';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case 't':
                                        {
                                            buffer[index] = '\t';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case 'n':
                                        {
                                            buffer[index] = '\n';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case 'f':
                                        {
                                            buffer[index] = '\f';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case 'r':
                                        {
                                            buffer[index] = '\r';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case '\\':
                                        {
                                            buffer[index] = '\\';
                                            index++;
                                            i += 1;
                                        }
                                        break;

                                    case 'u':
                                        {
                                            var j = i;
                                            if (j < 0)
                                                j = 0;

                                            if (objnodeValue.Length >= j + 5)
                                            {
                                                var @char = (new char[]
                                                {
                                                    objnodeValue[j + 0],
                                                    objnodeValue[j + 1],
                                                    objnodeValue[j + 2],
                                                    objnodeValue[j + 3],
                                                    objnodeValue[j + 4],
                                                    objnodeValue[j + 5],
                                                });

                                                var value = 0;
                                                var hex = 0;
                                                for (var ae = j + 2; ae < j + 6; ae++)
                                                {
                                                    /*0-9*/
                                                    if (objnodeValue[ae] >= 48 && objnodeValue[ae] <= 57)
                                                        hex = objnodeValue[ae] - 48;
                                                    /*A-F*/
                                                    else if (objnodeValue[ae] >= 65 && objnodeValue[ae] <= 70)
                                                        hex = objnodeValue[ae] - 55;
                                                    /*a-f*/
                                                    else if (objnodeValue[ae] >= 97 && objnodeValue[ae] <= 102)
                                                        hex = objnodeValue[ae] - 87;
                                                    else
                                                        throw new ArgumentException(string.Format("在读取{0}处字符失败了:不符合Unicode编码", ae.ToString()));

                                                    value += hex << (j + 5 - ae) * 4;
                                                }
                                                buffer[index] = Convert.ToChar(value);
                                                i = j + 5;
                                                index++;
                                            }
                                            else
                                            {
                                                buffer[index] = objnodeValue[i];
                                                index++;
                                            }
                                        }
                                        break;

                                    default:
                                        {
                                            buffer[index] = objnodeValue[i];
                                            index++;
                                        }
                                        break;
                                }
                            }
                            break;

                        default:
                            {
                                buffer[index] = objnodeValue[i];
                                index++;
                            }
                            break;
                    }
                }

                return new string(buffer, 0, index);
            }

            return objnode.ToString();
            //return reader.Read(objnode.Segment.Offset, objnode.Segment.Offset + objnode.Segment.Count);
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, string source)
        {
            /*要进行转义的*/
            if (source == null)
            {
                if (setting.WriteNullWhenObjectIsNull)
                    writer.Write("null");
                else
                    writer.Write("\"\"");
                return;
            }
            else if (source.Length == 0)
            {
                writer.Write("\"\"");
                return;
            }

            writer.Write("\"");
            for (var i = 0; i < source.Length; i++)
            {
                CharMethodProvider.Default.Write(writer, setting, source[i]);
            }
            writer.Write("\"");
        }

        #endregion IMethodProvider

        #region helper

        /// <summary>
        /// 前一个字符
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public char? Next(string value, int index)
        {
            if (index < 0)
                return null;

            if (index > value.Length - 2)
                return null;

            return value[index + 1];
        }

        /// <summary>
        /// 前一个字符
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public char? Next(char[] value, int index)
        {
            if (index < 0)
                return null;

            if (index > value.Length - 2)
                return null;

            return value[index + 1];
        }

        /// <summary>
        /// 前一个字符
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public char? Next(ArraySegmentValue value, int index)
        {
            if (index < 0)
                return null;

            if (index > value.Length - 2)
                return null;

            return value[index + 1];
        }
        #endregion helper
    }
}