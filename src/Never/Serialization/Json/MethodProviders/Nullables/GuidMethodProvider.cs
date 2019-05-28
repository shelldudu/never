using System;

namespace Never.Serialization.Json.MethodProviders.Nullables
{
    /// <summary>
    /// guid 值转换
    /// </summary>
    public class GuidMethodProvider : ConvertMethodProvider<Guid?>, IConvertMethodProvider<Guid?>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static GuidMethodProvider Default { get; } = new GuidMethodProvider();

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
        public override Guid? Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return null;

            if (checkNullValue && this.IsNullValue(value))
                return null;

            return MethodProviders.GuidMethodProvider.Default.Parse(reader,setting,  node, false);
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, Guid? source)
        {
            if (source.HasValue)
            {
                writer.Write("\"");
                MethodProviders.GuidMethodProvider.Default.Write(writer, setting, source.Value);
                writer.Write("\"");
                return;
            }

            writer.Write("null");
        }

        #endregion IMethodProvider
    }
}