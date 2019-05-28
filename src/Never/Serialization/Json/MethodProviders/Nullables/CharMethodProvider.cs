namespace Never.Serialization.Json.MethodProviders.Nullables
{
    /// <summary>
    /// char 值转换
    /// </summary>
    public class CharMethodProvider : ConvertMethodProvider<char?>, IConvertMethodProvider<char?>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static CharMethodProvider Default { get; } = new CharMethodProvider();

        #endregion singleton

        #region IMethodProvider

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="node">节点流内容</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override char? Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return null;

            if (checkNullValue && this.IsNullValue(value))
                return null;

            return MethodProviders.CharMethodProvider.Default.Parse(reader,setting,  node, false);
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, char? source)
        {
            if (source.HasValue)
            {
                writer.Write("\"");
                MethodProviders.CharMethodProvider.Default.Write(writer, setting, source.Value);
                writer.Write("\"");
                return;
            }

            writer.Write("null");
        }

        #endregion IMethodProvider
    }
}