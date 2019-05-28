using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// byte 值转换
    /// </summary>
    public class ByteMethodProvider : ConvertMethodProvider<byte>, IConvertMethodProvider<byte>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static ByteMethodProvider Default { get; } = new ByteMethodProvider();

        #endregion ctor

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="node">节点流内容</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override byte Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
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

                return ParseByte(value);
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
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte source)
        {
            char[] buffer = new char[3];
            var it = ZeroToHundred[source % 100];
            buffer[2] = it.Value;
            buffer[1] = it.Key;
            if (source < 10)
            {
                writer.Write(buffer, 2);
                return;
            }
            if (source < 100)
            {
                writer.Write(buffer, 1);
                return;
            }

            buffer[0] = ZeroToHundred[source / 100].Value;
            writer.Write(buffer);
            return;

            //char[] buffer = null;
            /*ushort的最大长度为5*/
            //var maxlength = "255".Length;
            //buffer = new char[maxlength + 1];
            //byte lessen = 0;
            //Tuple<char, char> item;
            //do
            //{
            //    lessen = (byte)(source % 100);
            //    item = ZeroToHundred[lessen];
            //    buffer[maxlength] = item.Item2;
            //    buffer[maxlength - 1] = item.Item1;
            //    maxlength -= 2;
            //    source = (byte)(source / 100);
            //} while (source != 0);

            //if (item.Item1 == '0')
            //{
            //    writer.Write(buffer, maxlength + 2);
            //    return;
            //}

            //writer.Write(buffer, maxlength + 1);
            //return;

            //var digitlength = GetDigitLength(source);
            //buffer = new char[digitlength];
            //WriteIntoChar(buffer, source, digitlength, 0);
            //writer.Write(buffer);
        }
    }
}