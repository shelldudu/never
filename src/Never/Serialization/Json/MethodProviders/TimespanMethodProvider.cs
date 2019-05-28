using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// timespan 值转换
    /// </summary>
    /// <remarks>
    /// TimeSpan.FromDays(1.2)
    /// 1.04:48:00
    /// </remarks>
    public class TimeSpanMethodProvider : ConvertMethodProvider<TimeSpan>, IConvertMethodProvider<TimeSpan>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static TimeSpanMethodProvider Default { get; } = new TimeSpanMethodProvider();

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
        public override TimeSpan Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return TimeSpan.Zero;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return TimeSpan.Zero;

            if (checkNullValue && this.IsNullValue(value))
                return TimeSpan.Zero;

            int point = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == ':')
                {
                    goto _noday;
                }

                if (value[i] == '.')
                    break;

                point++;
            }

            goto _hasday;

            _noday:
            {
                try
                {
                    point = 0;
                    int hour = 0;
                    if (value.Length >= point + 2)
                        hour = GetGigitInZeroToNightChar(value[0]) * 10 + GetGigitInZeroToNightChar(value[1]);
                    point += 3;

                    int minute = 0;
                    if (value.Length >= point + 2)
                        minute = GetGigitInZeroToNightChar(value[point]) * 10 + GetGigitInZeroToNightChar(value[point + 1]);
                    point += 3;

                    int second = 0;
                    if (value.Length >= point + 2)
                        second = GetGigitInZeroToNightChar(value[point]) * 10 + GetGigitInZeroToNightChar(value[point + 1]);
                    point += 3;

                    int mill = 0;
                    if (value.Length >= point + 3)
                        mill = GetGigitInZeroToNightChar(value[point]) * 100 + GetGigitInZeroToNightChar(value[point + 1]) * 10 + GetGigitInZeroToNightChar(value[point + 2]);

                    return new TimeSpan(0, hour, minute, second, mill);
                }
                catch (OverflowException)
                {
                    throw new ArgumentOutOfRangeException(node.Key, string.Format("数值溢出，在字符{0}至{1}处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                }
                catch
                {
                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                }
            };

            _hasday:
            {
                try
                {
                    int day = 0;
                    for (var i = 0; i < point; i++)
                    {
                        day += GetGigitInZeroToNightChar(value[i]) * (int)GetInteger(point - i);
                    }

                    int hour = 0;
                    if (value.Length >= point + 2)
                        hour = GetGigitInZeroToNightChar(value[point + 1]) * 10 + GetGigitInZeroToNightChar(value[point + 2]);
                    point += 3;

                    int minute = 0;
                    if (value.Length >= point + 2)
                        minute = GetGigitInZeroToNightChar(value[point + 1]) * 10 + GetGigitInZeroToNightChar(value[point + 2]);
                    point += 3;

                    int second = 0;
                    if (value.Length >= point + 2)
                        second = GetGigitInZeroToNightChar(value[point + 1]) * 10 + GetGigitInZeroToNightChar(value[point + 2]);
                    point += 3;

                    int mill = 0;
                    if (value.Length >= point + 3)
                        mill = GetGigitInZeroToNightChar(value[point + 1]) * 100 + GetGigitInZeroToNightChar(value[point + 2]) * 10 + GetGigitInZeroToNightChar(value[point + 3]);

                    return new TimeSpan(day, hour, minute, second, mill);
                }
                catch (OverflowException)
                {
                    throw new ArgumentOutOfRangeException(node.Key, string.Format("数值溢出，在字符{0}至{1}处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                }
                catch
                {
                    throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                }
            };
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, TimeSpan source)
        {
            char[] buffer = null;
            var item = new KeyValuePair<char, char>();
            if (source.Days == 0)
            {
                buffer = new char[8];
                item = ZeroToHundred[source.Hours];
                buffer[0] = item.Key;
                buffer[1] = item.Value;
                buffer[2] = ':';
                item = ZeroToHundred[source.Minutes];
                buffer[3] = item.Key;
                buffer[4] = item.Value;
                buffer[5] = ':';
                item = ZeroToHundred[source.Seconds];
                buffer[6] = item.Key;
                buffer[7] = item.Value;
                writer.Write(buffer);
                return;
            }

            var diglength = 1;
            if (source.Days < 10)
            {
                buffer = new char[10];
                buffer[0] = ZeroToHundred[source.Days].Value;
            }
            else
            {
                diglength = GetDigitLength(source.Days);
                var max = source.Days;
                var less = source.Days % 100;
                buffer = new char[diglength + 9];
                for (var i = diglength; i > 0; i = i - 2)
                {
                    item = ZeroToHundred[less];
                    if (less > 9)
                    {
                        buffer[i - 1] = item.Value;
                        buffer[i - 2] = item.Key;
                    }
                    else
                    {
                        buffer[i - 1] = item.Value;
                    }
                    max = max / 100;
                    less = max % 100;
                }
            }

            buffer[diglength] = '.';
            item = ZeroToHundred[source.Hours];
            buffer[diglength + 1] = item.Key;
            buffer[diglength + 2] = item.Value;
            buffer[diglength + 3] = ':';
            item = ZeroToHundred[source.Minutes];
            buffer[diglength + 4] = item.Key;
            buffer[diglength + 5] = item.Value;
            buffer[diglength + 6] = ':';
            item = ZeroToHundred[source.Seconds];
            buffer[diglength + 7] = item.Key;
            buffer[diglength + 8] = item.Value;

            writer.Write(buffer);
        }

        #endregion IMethodProvider
    }
}