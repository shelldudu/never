namespace Never.Serialization.Json.MethodProviders.Nullables
{
    /// <summary>
    /// int 值转换
    /// </summary>
    public class Int32MethodProvider : ConvertMethodProvider<int?>, IConvertMethodProvider<int?>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static Int32MethodProvider Default { get; } = new Int32MethodProvider();

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
        public override int? Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return null;

            if (checkNullValue && this.IsNullValue(value))
                return null;

            return MethodProviders.Int32MethodProvider.Default.Parse(reader,setting,  node, false);
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, int? source)
        {
            if (source.HasValue)
            {
                if (setting.WriteQuoteWhenObjectIsNumber)
                {
                    writer.Write("\"");
                    MethodProviders.Int32MethodProvider.Default.Write(writer, setting, source.Value);
                    writer.Write("\"");
                }
                else
                {
                    MethodProviders.Int32MethodProvider.Default.Write(writer, setting, source.Value);
                }

                return;
            }

            writer.Write("null");
        }

        #endregion IMethodProvider
    }
}