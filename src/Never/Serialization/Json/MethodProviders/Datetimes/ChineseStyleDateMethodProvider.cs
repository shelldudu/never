using System;

namespace Never.Serialization.Json.MethodProviders.DateTimes
{
    /// <summary>
    /// datetime 值转换
    /// </summary>
    public class ChineseStyleDateMethodProvider : DateTimeMethodProvider
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static new ChineseStyleDateMethodProvider Default { get; } = new ChineseStyleDateMethodProvider();

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="node">节点流内容</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public override DateTime Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return new DateTime();

            var value = node.GetValue();
            try
            {
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, mill = 0;
                if (value.Length > 22)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                    day = GetGigitInZeroToNightChar(value[8]) * 10 + GetGigitInZeroToNightChar(value[9]);
                    hour = GetGigitInZeroToNightChar(value[11]) * 10 + GetGigitInZeroToNightChar(value[12]);
                    minute = GetGigitInZeroToNightChar(value[14]) * 10 + GetGigitInZeroToNightChar(value[15]);
                    second = GetGigitInZeroToNightChar(value[17]) * 10 + GetGigitInZeroToNightChar(value[18]);
                    mill = GetGigitInZeroToNightChar(value[20]) * 100 + GetGigitInZeroToNightChar(value[21]) * 10 + GetGigitInZeroToNightChar(value[22]);
                }
                else if (value.Length > 18)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                    day = GetGigitInZeroToNightChar(value[8]) * 10 + GetGigitInZeroToNightChar(value[9]);
                    hour = GetGigitInZeroToNightChar(value[11]) * 10 + GetGigitInZeroToNightChar(value[12]);
                    minute = GetGigitInZeroToNightChar(value[14]) * 10 + GetGigitInZeroToNightChar(value[15]);
                    second = GetGigitInZeroToNightChar(value[17]) * 10 + GetGigitInZeroToNightChar(value[18]);
                }
                else if (value.Length > 15)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                    day = GetGigitInZeroToNightChar(value[8]) * 10 + GetGigitInZeroToNightChar(value[9]);
                    hour = GetGigitInZeroToNightChar(value[11]) * 10 + GetGigitInZeroToNightChar(value[12]);
                    minute = GetGigitInZeroToNightChar(value[14]) * 10 + GetGigitInZeroToNightChar(value[15]);
                }
                else if (value.Length > 12)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                    day = GetGigitInZeroToNightChar(value[8]) * 10 + GetGigitInZeroToNightChar(value[9]);
                    hour = GetGigitInZeroToNightChar(value[11]) * 10 + GetGigitInZeroToNightChar(value[12]);
                }
                else if (value.Length > 9)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                    day = GetGigitInZeroToNightChar(value[8]) * 10 + GetGigitInZeroToNightChar(value[9]);
                }
                else if (value.Length > 6)
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                    month = GetGigitInZeroToNightChar(value[5]) * 10 + GetGigitInZeroToNightChar(value[6]);
                }
                else
                {
                    year = GetGigitInZeroToNightChar(value[0]) * 1000 + GetGigitInZeroToNightChar(value[1]) * 100 + GetGigitInZeroToNightChar(value[2]) * 10 + GetGigitInZeroToNightChar(value[3]);
                }

                return new DateTime(year, month, day, hour, minute, second, mill);
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
            char[] buffer = null;
            buffer = new char[23];
            var item = ZeroToHundred[source.Year % 100];
            buffer[2] = item.Key;
            buffer[3] = item.Value;
            item = ZeroToHundred[source.Year / 100];
            buffer[0] = item.Key;
            buffer[1] = item.Value;

            buffer[4] = '-';

            item = ZeroToHundred[source.Month];
            buffer[5] = item.Key;
            buffer[6] = item.Value;

            buffer[7] = '-';

            item = ZeroToHundred[source.Day];
            buffer[8] = item.Key;
            buffer[9] = item.Value;

            buffer[10] = ' ';

            item = ZeroToHundred[source.Hour];
            buffer[11] = item.Key;
            buffer[12] = item.Value;

            buffer[13] = ':';

            item = ZeroToHundred[source.Minute];
            buffer[14] = item.Key;
            buffer[15] = item.Value;

            buffer[16] = ':';

            item = ZeroToHundred[source.Second];
            buffer[17] = item.Key;
            buffer[18] = item.Value;

            buffer[19] = '.';

            item = ZeroToHundred[source.Millisecond % 100];
            buffer[20] = item.Key;
            buffer[21] = item.Value;
            item = ZeroToHundred[source.Millisecond / 100];
            buffer[22] = item.Key;

            writer.Write(buffer);
            return;
        }

        #endregion IMethodProvider
    }
}