using System;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// guid 值转换
    /// </summary>
    public class GuidMethodProvider : ConvertMethodProvider<Guid>, IConvertMethodProvider<Guid>
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
        public override Guid Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            if (node == null)
                return Guid.Empty;

            var value = node.GetValue();
            if (value.IsNullOrEmpty)
                return Guid.Empty;

            if (checkNullValue && this.IsNullValue(value))
                return Guid.Empty;

            if (value.Length == 32)
            {
                /*9c43f081-a003-4b6f-b1fd-a5e8017c6b2b*/
                var buffer = new char[36];
                for (var i = 0; i < 8; i++)
                {
                    buffer[i] = value[i];
                }

                buffer[8] = '-';

                for (var i = 9; i < 13; i++)
                {
                    buffer[i] = value[i - 1];
                }

                buffer[13] = '-';

                for (var i = 14; i < 18; i++)
                {
                    buffer[i] = value[i - 2];
                }

                buffer[18] = '-';

                for (var i = 19; i < 23; i++)
                {
                    buffer[i] = value[i - 3];
                }

                buffer[23] = '-';

                for (var i = 24; i < 36; i++)
                {
                    buffer[i] = value[i - 4];
                }

                return new Guid(new string(buffer));
            }

            if (value.Length == 36)
            {
                if (value[8] != '-' || value[13] != '-' || value[18] != '-' || value[23] != '-')
                    throw new ArgumentException("字符中处在8,13,18,23索引的值应该为'-'");

                return new Guid(value.ToString());
            }

            return Guid.Empty;
        }

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, Guid source)
        {
            var guid = source.ToString();
            if (setting.WriteHorizontalLineOnGuidType)
            {
                writer.Write(guid);
                return;
            }

            /*9c43f081-a003-4b6f-b1fd-a5e8017c6b2b*/

            var buffer = new char[32];
            for (var i = 0; i < 8; i++)
            {
                buffer[i] = guid[i];
            }

            for (var i = 9; i < 13; i++)
            {
                buffer[i - 1] = guid[i];
            }
            for (var i = 14; i < 18; i++)
            {
                buffer[i - 2] = guid[i];
            }
            for (var i = 19; i < 23; i++)
            {
                buffer[i - 3] = guid[i];
            }
            for (var i = 24; i < 36; i++)
            {
                buffer[i - 4] = guid[i];
            }

            writer.Write(buffer);
        }

        #endregion IMethodProvider
    }
}