using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// long 值转换
    /// </summary>
    public class Int64MethodProvider : ConvertMethodProvider<long>, IConvertMethodProvider<long>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static Int64MethodProvider Default { get; } = new Int64MethodProvider();

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
        public override long Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return 0;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return 0;

            try
            {
                if (checkNullValue && this.IsNullValue(value))
                    return 0;

                return ParseInt64(value);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentNullException(string.Format("读取发现错误，在{0}至{1}字符处，取值只能在[0-9]范围内", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException(node.Key, string.Format("数值溢出，在字符{0}至{1}处", node.Segment.Offset.ToString(), (node.Segment.Offset + node.Segment.Count).ToString()));
            }
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, long source)
        {
            if (source == long.MinValue)
            {
                writer.Write("-9223372036854775808");
                return;
            }

            if (source == long.MaxValue)
            {
                writer.Write("9223372036854775807");
                return;
            }

            var target = source;
            if (source < 0)
            {
                writer.Write('-');
                target = -target;
            }

            char[] buffer = null;
            /*long的最大长度为19，加一个负数符号*/
            var maxlength = "9223372036854775807".Length;
            buffer = new char[maxlength + 1];
            var lessen = 0L;
            KeyValuePair<char, char> item;
            do
            {
                lessen = target % 100;
                item = ZeroToHundred[lessen];
                buffer[maxlength] = item.Value;
                buffer[maxlength - 1] = item.Key;
                maxlength -= 2;
                target = (target / 100);
            } while (target != 0);

            if (item.Key == '0')
            {
                writer.Write(buffer, maxlength + 2);
                return;
            }

            writer.Write(buffer, maxlength + 1);
            return;

            //var digitlength = GetDigitLength(source);
            //buffer = new char[digitlength];
            //WriteIntoChar(buffer, source, digitlength, 0);
            //writer.Write(buffer);
        }

        #endregion IMethodProvider
    }
}