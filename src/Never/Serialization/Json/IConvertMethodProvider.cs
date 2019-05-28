namespace Never.Serialization.Json
{
    /// <summary>
    /// 转换方法提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConvertMethodProvider<T>
    {
        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source"></param>
        void Write(ISerializerWriter writer, JsonSerializeSetting setting, T source);

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="node">节点流内容</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        T Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue);
    }
}