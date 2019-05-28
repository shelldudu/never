using Never.Serialization.Json.MethodProviders.DateTimes;
using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// datetime 值转换
    /// </summary>
    public class DateTimeMethodProvider : ConvertMethodProvider<DateTime>, IConvertMethodProvider<DateTime>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static DateTimeMethodProvider Default { get; } = new DateTimeMethodProvider();

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
        public override DateTime Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return new DateTime();

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return new DateTime();

            if (checkNullValue && this.IsNullValue(value))
                return new DateTime();

            if (value[value.Length - 1] == 'Z')
                return ISO8601StyleDateMethodProvider.Default.Parse(reader, setting, node, checkNullValue);

            if (value[value.Length - 1] == 'T')
                return RFC1123StyleDateMethodProvider.Default.Parse(reader, setting, node, checkNullValue);

            if (value[value.Length - 1] == '/')
                return MicrosoftStyleDateMethodProvider.Default.Parse(reader, setting, node, checkNullValue);

            if (value.Length > 10)
            {
                if (value[10] == ' ' || value[9] == ' ' || value[8] == ' ')
                    return ChineseStyleDateMethodProvider.Default.Parse(reader, setting, node, checkNullValue);
            }

            try
            {
                DateTime time;
                DateTime.TryParse(value.ToString(), out time);
                return time;
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
            writer.Write(source.ToString());
        }

        #endregion IMethodProvider
    }
}