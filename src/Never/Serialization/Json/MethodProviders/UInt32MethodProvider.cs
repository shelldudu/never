using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// uint 值转换
    /// </summary>
    public class UInt32MethodProvider : ConvertMethodProvider<uint>, IConvertMethodProvider<uint>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static UInt32MethodProvider Default { get; } = new UInt32MethodProvider();

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
        public override uint Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
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

                return ParseUInt32(value);
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
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, uint source)
        {
            if (source == uint.MinValue)
            {
                writer.Write("0");
                return;
            }

            if (source == uint.MaxValue)
            {
                writer.Write("4294967295");
                return;
            }

            char[] buffer = null;
            /*ushort的最大长度为10*/
            var maxlength = "4294967295".Length;
            buffer = new char[maxlength + 1];
            ulong lessen = 0;
            KeyValuePair<char, char> item;
            do
            {
                lessen = source % 100;
                item = ZeroToHundred[lessen];
                buffer[maxlength] = item.Value;
                buffer[maxlength - 1] = item.Key;
                maxlength -= 2;
                source = source / 100;
            } while (source != 0);

            if (item.Key == '0')
            {
                writer.Write(buffer, maxlength + 2);
                return;
            }

            writer.Write(buffer, maxlength + 1);
            return;
        }

        #endregion IMethodProvider
    }
}