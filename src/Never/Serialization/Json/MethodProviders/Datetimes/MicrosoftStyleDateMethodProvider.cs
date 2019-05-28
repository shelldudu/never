using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.MethodProviders.DateTimes
{
    /// <summary>
    /// datetime 值转换
    /// </summary>
    public class MicrosoftStyleDateMethodProvider : DateTimeMethodProvider
    {
        #region ctor

        private static readonly DateTime _1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static new MicrosoftStyleDateMethodProvider Default { get; } = new MicrosoftStyleDateMethodProvider();

        #endregion singleton

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
                double ret = 0d;
                for (var i = 7; i <= 19; i++)
                {
                    switch (value[i])
                    {
                        case '0':
                            {
                            }
                            break;

                        case '1':
                            {
                                ret += GetInteger(20 - i);
                            }
                            break;

                        case '2':
                            {
                                ret += 2 * GetInteger(20 - i);
                            }
                            break;

                        case '3':
                            {
                                ret += 3 * GetInteger(20 - i);
                            }
                            break;

                        case '4':
                            {
                                ret += 4 * GetInteger(20 - i);
                            }
                            break;

                        case '5':
                            {
                                ret += 5 * GetInteger(20 - i);
                            }
                            break;

                        case '6':
                            {
                                ret += 6 * GetInteger(20 - i);
                            }
                            break;

                        case '7':
                            {
                                ret += 7 * GetInteger(20 - i);
                            }
                            break;

                        case '8':
                            {
                                ret += 8 * GetInteger(20 - i);
                            }
                            break;

                        case '9':
                            {
                                ret += 9 * GetInteger(20 - i);
                            }
                            break;

                        default:
                            throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在[0,9]范围内", (node.Segment.Offset).ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
                    }
                }

                return _1970.AddMilliseconds(ret);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(node.Key, string.Format("数值溢出，在字符{0}至{1}处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
            }
            catch
            {
                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在[0-9]范围内", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
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
            var timespan = source.ToUniversalTime() - _1970;
            KeyValuePair<char, char> item;
            /*long的最大长度为20,加预留字段10个*/
            var maxlength = 26;
            buffer = new char[30];
            buffer[27] = ')';
            buffer[28] = '\\';
            buffer[29] = '/';
            var target = (long)timespan.TotalMilliseconds;
            var lessen = 0L;
            do
            {
                lessen = target % 100;
                item = ZeroToHundred[lessen];
                buffer[maxlength] = item.Value;
                buffer[maxlength - 1] = item.Key;
                maxlength -= 2;
                target = target / 100;
            } while (target > 10);

            if (target == 0)
            {
                buffer[maxlength - 6] = '\\';
                buffer[maxlength - 5] = '/';
                buffer[maxlength - 4] = 'D';
                buffer[maxlength - 3] = 'a';
                buffer[maxlength - 2] = 't';
                buffer[maxlength - 1] = 'e';
                buffer[maxlength] = '(';
                writer.Write(buffer, maxlength - 6);
                return;
            }

            buffer[maxlength - 7] = '\\';
            buffer[maxlength - 6] = '/';
            buffer[maxlength - 5] = 'D';
            buffer[maxlength - 4] = 'a';
            buffer[maxlength - 3] = 't';
            buffer[maxlength - 2] = 'e';
            buffer[maxlength - 1] = '(';
            buffer[maxlength] = ZeroToHundred[target].Value;
            writer.Write(buffer, maxlength - 7);
            return;

            /*old算法*/
            //var max = (long)timespan.TotalMilliseconds;
            //var diglength = GetDigitLength(max);
            //var less = max % 100;
            //* 7 + length + 3*/
            //buffer = new char[diglength + 10];
            //buffer[0] = '\\';
            //buffer[1] = '/';
            //buffer[2] = 'D';
            //buffer[3] = 'a';
            //buffer[4] = 't';
            //buffer[5] = 'e';
            //buffer[6] = '(';

            /*long max length 20*/
            //for (var i = diglength + 6; i > 6; i = i - 2)
            //{
            //    item = ZeroToHundred[less];
            //    buffer[i] = item.Item2;
            //    buffer[i - 1] = item.Item1;
            //    max = max / 100;
            //    less = max % 100;
            //}

            /*因为上面循环条件中i>6,表示i的最小值为7，有可能会写了6的值，但能写6的值通常是是\0这个值
            // 为什么是\0这个值，由于max的长度已经计算出来，所以在分配的时候长度固定，max在buffer中开始的索引是从7开始的，一直到长度 + 6，%100会表示每次取2位值，到了less为1-9的数，在ZeroToHundred中1-9是\0\n开头的
            // */
            //buffer[6] = '(';
            //buffer[diglength + 7] = ')';
            //buffer[diglength + 8] = '\\';
            //buffer[diglength + 9] = '/';

            //writer.Write(buffer);
        }

        #endregion IMethodProvider
    }
}