namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// bool 值转换
    /// </summary>
    public class BooleanMethodProvider : ConvertMethodProvider<bool>, IConvertMethodProvider<bool>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static BooleanMethodProvider Default { get; } = new BooleanMethodProvider();

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
        public override bool Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return false;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return false;

            if (value.Length == 1)
                return value[0] == '1';

            if (IsNullValue(value))
                return false;

            if (value.Length != 4)
                return false;

            return (value[0] == 't' || value[0] == 'T')
                    & (value[1] == 'r' || value[1] == 'R')
                    & (value[2] == 'u' || value[2] == 'U')
                    & (value[3] == 'e' || value[3] == 'e');
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, bool source)
        {
            if (setting.WriteNumberOnBoolenType)
            {
                writer.Write(source ? "1" : "0");
            }
            else
            {
                writer.Write(source ? "true" : "false");
            }
            return;
        }

        #endregion IMethodProvider
    }
}