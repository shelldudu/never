using System;

namespace Never.Serialization.Json.MethodProviders.DateTimes
{
    /// <summary>
    /// datetime 值转换，时间格式为：Thu, 10 Apr 2008 13:30:00 GMT
    /// </summary>
    public class RFC1123StyleDateMethodProvider : DateTimeMethodProvider
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static new RFC1123StyleDateMethodProvider Default { get; } = new RFC1123StyleDateMethodProvider();

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
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public override DateTime Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return new DateTime();

            var value = node.GetValue();
            try
            {
                if (value.IsNullOrEmpty)
                    return new DateTime();

                /*空格位置*/
                int space = 3;
                /*获取天数*/
                for (var i = 3; i < value.Length; i++)
                {
                    if (value[i] == ' ')
                        break;

                    space++;
                }

                int day = GetGigitInZeroToNightChar(value[space + 1]) * 10 + GetGigitInZeroToNightChar(value[space + 2]);
                space += 2;

                /*获取月份*/
                for (var i = space; i < value.Length; i++)
                {
                    if (value[i] == ' ')
                        break;

                    space++;
                }

                int month = 0;

                #region month

                switch (value[space + 1])
                {
                    case 'j':
                    case 'J':
                        {
                            switch (value[space + 2])
                            {
                                case 'a':
                                case 'A':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'n':
                                            case 'N':
                                                {
                                                    month = 1;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（nN）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                case 'u':
                                case 'U':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'n':
                                            case 'N':
                                                {
                                                    month = 6;
                                                }
                                                break;

                                            case 'l':
                                            case 'L':
                                                {
                                                    month = 7;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（nN或lL）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（aA或uU）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'F':
                    case 'f':
                        {
                            switch (value[space + 2])
                            {
                                case 'e':
                                case 'E':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'b':
                                            case 'B':
                                                {
                                                    month = 2;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（bB）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（eE）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'M':
                    case 'm':
                        {
                            switch (value[space + 2])
                            {
                                case 'a':
                                case 'A':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'r':
                                            case 'R':
                                                {
                                                    month = 3;
                                                }
                                                break;

                                            case 'y':
                                            case 'Y':
                                                {
                                                    month = 5;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（rR或yY）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（aA）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'A':
                    case 'a':
                        {
                            switch (value[space + 2])
                            {
                                case 'p':
                                case 'P':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'r':
                                            case 'R':
                                                {
                                                    month = 4;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（rR）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                case 'U':
                                case 'u':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'g':
                                            case 'G':
                                                {
                                                    month = 8;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（gG）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（pPUu）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'S':
                    case 's':
                        {
                            switch (value[space + 2])
                            {
                                case 'e':
                                case 'E':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'p':
                                            case 'P':
                                                {
                                                    month = 9;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（pP）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（eE）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'O':
                    case 'o':
                        {
                            switch (value[space + 2])
                            {
                                case 'c':
                                case 'C':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 't':
                                            case 'T':
                                                {
                                                    month = 10;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（tT）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（cC）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'N':
                    case 'n':
                        {
                            switch (value[space + 2])
                            {
                                case 'o':
                                case 'O':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'v':
                                            case 'V':
                                                {
                                                    month = 11;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（vV）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（oO）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;

                    case 'D':
                    case 'd':
                        {
                            switch (value[space + 2])
                            {
                                case 'e':
                                case 'E':
                                    {
                                        switch (value[space + 3])
                                        {
                                            case 'c':
                                            case 'C':
                                                {
                                                    month = 12;
                                                }
                                                break;

                                            default:
                                                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（cC）范围内", (node.Segment.Offset + 2).ToString(), (node.Segment.Offset + 2).ToString()));
                                        }
                                    }
                                    break;

                                default:
                                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在（eE）范围内", (node.Segment.Offset + 1).ToString(), (node.Segment.Offset + 1).ToString()));
                            }
                        }
                        break;
                }

                #endregion month

                /*获取年份*/
                space += 3;
                for (var i = space; i < value.Length; i++)
                {
                    if (value[i] == ' ')
                        break;

                    space++;
                }
                int year = GetGigitInZeroToNightChar(value[space + 1]) * 1000 + GetGigitInZeroToNightChar(value[space + 2]) * 100 + GetGigitInZeroToNightChar(value[space + 3]) * 10 + GetGigitInZeroToNightChar(value[space + 4]);

                /*获取小时*/
                space += 4;
                for (var i = space; i < value.Length; i++)
                {
                    if (value[i] == ' ')
                        break;

                    space++;
                }

                int hour = GetGigitInZeroToNightChar(value[space + 1]) * 10 + GetGigitInZeroToNightChar(value[space + 2]);

                /*获取分钟*/
                int minute = GetGigitInZeroToNightChar(value[space + 4]) * 10 + GetGigitInZeroToNightChar(value[space + 5]);

                /*获取秒数*/
                int second = GetGigitInZeroToNightChar(value[space + 7]) * 10 + GetGigitInZeroToNightChar(value[space + 8]);

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(node.Key, string.Format("数值溢出，在字符{0}至{1}处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
            }
            catch
            {
                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
            }
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source"></param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, DateTime source)
        {
            //Thu, 10 Apr 2008 13:30:00 GMT
            char[] buffer = new char[29];
            switch (source.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    {
                        buffer[0] = 'F';
                        buffer[1] = 'r';
                        buffer[2] = 'i';
                    }
                    break;

                case DayOfWeek.Monday:
                    {
                        buffer[0] = 'M';
                        buffer[1] = 'o';
                        buffer[2] = 'n';
                    }
                    break;

                case DayOfWeek.Saturday:
                    {
                        buffer[0] = 'S';
                        buffer[1] = 'a';
                        buffer[2] = 't';
                    }
                    break;

                case DayOfWeek.Sunday:
                    {
                        buffer[0] = 'S';
                        buffer[1] = 'u';
                        buffer[2] = 'n';
                    }
                    break;

                case DayOfWeek.Thursday:
                    {
                        buffer[0] = 'T';
                        buffer[1] = 'h';
                        buffer[2] = 'u';
                    }
                    break;

                case DayOfWeek.Tuesday:
                    {
                        buffer[0] = 'T';
                        buffer[1] = 'u';
                        buffer[2] = 's';
                    }
                    break;

                case DayOfWeek.Wednesday:
                    {
                        buffer[0] = 'W';
                        buffer[1] = 'e';
                        buffer[2] = 'd';
                    }
                    break;
            }
            buffer[3] = ',';
            buffer[4] = ' ';
            var item = ZeroToHundred[source.Month];
            buffer[5] = item.Key;
            buffer[6] = item.Value;
            buffer[7] = ' ';
            switch (source.Month)
            {
                case 1:
                    {
                        buffer[8] = 'J';
                        buffer[9] = 'a';
                        buffer[10] = 'n';
                    }
                    break;

                case 2:
                    {
                        buffer[8] = 'F';
                        buffer[9] = 'e';
                        buffer[10] = 'b';
                    }
                    break;

                case 3:
                    {
                        buffer[8] = 'M';
                        buffer[9] = 'a';
                        buffer[10] = 'r';
                    }
                    break;

                case 4:
                    {
                        buffer[8] = 'A';
                        buffer[9] = 'p';
                        buffer[10] = 'r';
                    }
                    break;

                case 5:
                    {
                        buffer[8] = 'M';
                        buffer[9] = 'a';
                        buffer[10] = 'y';
                    }
                    break;

                case 6:
                    {
                        buffer[8] = 'J';
                        buffer[9] = 'u';
                        buffer[10] = 'n';
                    }
                    break;

                case 7:
                    {
                        buffer[8] = 'J';
                        buffer[9] = 'u';
                        buffer[10] = 'l';
                    }
                    break;

                case 8:
                    {
                        buffer[8] = 'A';
                        buffer[9] = 'u';
                        buffer[10] = 'g';
                    }
                    break;

                case 9:
                    {
                        buffer[8] = 'S';
                        buffer[9] = 'e';
                        buffer[10] = 'p';
                    }
                    break;

                case 10:
                    {
                        buffer[8] = 'O';
                        buffer[9] = 'c';
                        buffer[10] = 't';
                    }
                    break;

                case 11:
                    {
                        buffer[8] = 'N';
                        buffer[9] = 'o';
                        buffer[10] = 'v';
                    }
                    break;

                case 12:
                    {
                        buffer[8] = 'D';
                        buffer[9] = 'e';
                        buffer[10] = 'c';
                    }
                    break;
            }

            buffer[11] = ' ';
            item = ZeroToHundred[source.Year % 100];
            buffer[14] = item.Key;
            buffer[15] = item.Value;
            item = ZeroToHundred[source.Year / 100];
            buffer[12] = item.Key;
            buffer[13] = item.Value;

            buffer[16] = ' ';
            item = ZeroToHundred[source.Hour];
            buffer[17] = item.Key;
            buffer[18] = item.Value;
            buffer[19] = ':';
            item = ZeroToHundred[source.Minute];
            buffer[20] = item.Key;
            buffer[21] = item.Value;
            buffer[22] = ':';
            item = ZeroToHundred[source.Second];
            buffer[23] = item.Key;
            buffer[24] = item.Value;
            buffer[25] = ' ';
            buffer[26] = 'G';
            buffer[27] = 'M';
            buffer[28] = 'T';

            writer.Write(buffer);
        }

        #endregion IMethodProvider
    }
}