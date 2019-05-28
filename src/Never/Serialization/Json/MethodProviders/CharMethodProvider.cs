using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// char 值转换
    /// </summary>
    public class CharMethodProvider : ConvertMethodProvider<char>, IConvertMethodProvider<char>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static CharMethodProvider Default { get; } = new CharMethodProvider();

        /// <summary>
        /// 
        /// </summary>
        private static readonly char[] hexChars = new char[]
        {
            '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F','a','b','c','d','e','f'
        };

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
        public override char Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return default(char);

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return default(char);

            if (checkNullValue && this.IsNullValue(value))
                return ' ';

            if (value.Length == 1)
                return value[0];

            if (value.Length == 2)
                return value[1];

            if (value[1] != 'u')
                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，char取值只能一位", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));

            if (value.Length != 6)
                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，char取值只能一位", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));

            var ret = 0;
            for (var i = 2; i < 6; i++)
            {
                switch (value[i])
                {
                    case '0':
                        {
                        }
                        break;

                    case '1':
                        {
                            ret += _16Pow(5 - i);
                        }
                        break;

                    case '2':
                        {
                            ret += 2 * _16Pow(5 - i);
                        }
                        break;

                    case '3':
                        {
                            ret += 3 * _16Pow(5 - i);
                        }
                        break;

                    case '4':
                        {
                            ret += 4 * _16Pow(5 - i);
                        }
                        break;

                    case '5':
                        {
                            ret += 5 * _16Pow(5 - i);
                        }
                        break;

                    case '6':
                        {
                            ret += 6 * _16Pow(5 - i);
                        }
                        break;

                    case '7':
                        {
                            ret += 7 * _16Pow(5 - i);
                        }
                        break;

                    case '8':
                        {
                            ret += 8 * _16Pow(5 - i);
                        }
                        break;

                    case '9':
                        {
                            ret += 9 * _16Pow(5 - i);
                        }
                        break;

                    case 'a':
                    case 'A':
                        {
                            ret += 10 * _16Pow(5 - i);
                        }
                        break;

                    case 'b':
                    case 'B':
                        {
                            ret += 11 * _16Pow(5 - i);
                        }
                        break;

                    case 'c':
                    case 'C':
                        {
                            ret += 12 * _16Pow(5 - i);
                        }
                        break;

                    case 'd':
                    case 'D':
                        {
                            ret += 13 * _16Pow(5 - i);
                        }
                        break;

                    case 'e':
                    case 'E':
                        {
                            ret += 14 * _16Pow(5 - i);
                        }
                        break;

                    case 'f':
                    case 'F':
                        {
                            ret += 15 * _16Pow(5 - i);
                        }
                        break;

                    default:
                        {
                            throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，char取值只能一位", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                        }
                }
            }

            return (char)ret;
        }

        /// <summary>
        /// 16的幂
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static int _16Pow(int time)
        {
            switch (time)
            {
                case 0:
                    {
                        return 1;
                    }
                case 1:
                    {
                        return 16;
                    }
                case 2:
                    {
                        return 256;
                    }
                case 3:
                    {
                        return 4096;
                    }
            }

            return 1;
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, char source)
        {
            //var str = new string(new[] { source });
            //characterEscapes.TryGetValue(source, out str);
            //writer.Write(str);
            //return;

            if (setting.WriteWithUnicodeOnStringType)
            {
                writer.Write("\\u");
                writer.Write(hexChars[(source >> 12) & 15]);
                writer.Write(hexChars[(source >> 8) & 15]);
                writer.Write(hexChars[(source >> 4) & 15]);
                writer.Write(hexChars[(source) & 15]);
                return;
            }

            switch (source)
            {
                case '\u0000':
                    {
                        writer.Write(@"\u0000");
                        return;
                    }
                case '\u0001':
                    {
                        writer.Write(@"\u0001");
                        return;
                    }
                case '\u0002':
                    {
                        writer.Write(@"\u0002");
                        return;
                    }
                case '\u0003':
                    {
                        writer.Write(@"\u0003");
                        return;
                    }
                case '\u0004':
                    {
                        writer.Write(@"\u0004");
                        return;
                    }
                case '\u0005':
                    {
                        writer.Write(@"\u0005");
                        return;
                    }
                case '\u0006':
                    {
                        writer.Write(@"\u0006");
                        return;
                    }
                case '\u0007':
                    {
                        writer.Write(@"\u0007");
                        return;
                    }
                case '\u0008':
                    {
                        writer.Write(@"\b");
                        return;
                    }
                case '\u0009':
                    {
                        writer.Write(@"\t");
                        return;
                    }
                case '\u000A':
                    {
                        writer.Write(@"\n");
                        return;
                    }
                case '\u000B':
                    {
                        writer.Write(@"\u000b");
                        return;
                    }
                case '\u000C':
                    {
                        writer.Write(@"\f");
                        return;
                    }
                case '\u000D':
                    {
                        writer.Write(@"\r");
                        return;
                    }
                case '\u000E':
                    {
                        writer.Write(@"\u000e");
                        return;
                    }
                case '\u000F':
                    {
                        writer.Write(@"\u000f");
                        return;
                    }
                case '\u0010':
                    {
                        writer.Write(@"\u0010");
                        return;
                    }
                case '\u0011':
                    {
                        writer.Write(@"\u0011");
                        return;
                    }
                case '\u0012':
                    {
                        writer.Write(@"\u0012");
                        return;
                    }
                case '\u0013':
                    {
                        writer.Write(@"\u0013");
                        return;
                    }
                case '\u0014':
                    {
                        writer.Write(@"\u0014");
                        return;
                    }
                case '\u0015':
                    {
                        writer.Write(@"\u0015");
                        return;
                    }
                case '\u0016':
                    {
                        writer.Write(@"\u0016");
                        return;
                    }
                case '\u0017':
                    {
                        writer.Write(@"\u0017");
                        return;
                    }
                case '\u0018':
                    {
                        writer.Write(@"\u0018");
                        return;
                    }
                case '\u0019':
                    {
                        writer.Write(@"\u0019");
                        return;
                    }
                case '\u001A':
                    {
                        writer.Write(@"\u001a");
                        return;
                    }
                case '\u001B':
                    {
                        writer.Write(@"\u001b");
                        return;
                    }
                case '\u001C':
                    {
                        writer.Write(@"\u001c");
                        return;
                    }
                case '\u001D':
                    {
                        writer.Write(@"\u001d");
                        return;
                    }
                case '\u001E':
                    {
                        writer.Write(@"\u001e");
                        return;
                    }
                case '\u001F':
                    {
                        writer.Write(@"\u001f");
                        return;
                    }
                case '\u0022':
                    {
                        writer.Write("\\\"");
                        return;
                    }
                case '\u0027':
                    {
                        writer.Write(@"'");
                        return;
                    }
                case '\u005C':
                    {
                        writer.Write(@"\\");
                        return;
                    }
            }

            writer.Write(source);
        }

        #endregion IMethodProvider
    }
}