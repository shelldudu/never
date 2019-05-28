using System;
using System.Text.RegularExpressions;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// type 值转换
    /// </summary>
    public class TypeMethodProvider : ConvertMethodProvider<Type>, IConvertMethodProvider<Type>
    {
        #region ctor

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static TypeMethodProvider Default { get; } = new TypeMethodProvider();

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
        public override Type Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return null;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return null;

            try
            {
                if (checkNullValue && this.IsNullValue(value))
                    return null;

                return Type.GetType(value.ToString());
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
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, Type source)
        {
            if (setting.WriteVersionOnTypeInfo)
            {
                writer.Write(source.AssemblyQualifiedName);
            }
            else
            {
                writer.Write(Regex.Replace(source.AssemblyQualifiedName, "\\s+version=(.*?),", "", RegexOptions.IgnoreCase));
            }
        }

        #endregion IMethodProvider
    }
}