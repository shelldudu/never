using System;
using System.Text.RegularExpressions;

namespace Never.Messages
{
    /// <summary>
    /// 报文信息
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public sealed class MessagePacket
    {
        #region prop

        /// <summary>
        /// 消息对象的具体类型
        /// </summary>
        [Serialization.Json.DataMember(Name = "ContentType")]
        [System.Runtime.Serialization.DataMember(Name = "ContentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// 消息对象的主体,通常是json对象
        /// </summary>
        [Serialization.Json.DataMember(Name = "Body")]
        [System.Runtime.Serialization.DataMember(Name = "Body")]
        public string Body { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes the <see cref="MessagePacket"/> class.
        /// </summary>
        static MessagePacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket"/> class.
        /// </summary>
        public MessagePacket()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket"/> class.
        /// </summary>
        /// <param name="contentType">The contentType.</param>
        /// <param name="body">The body.</param>
        public MessagePacket(string contentType, string body)
        {
            this.ContentType = contentType;
            this.Body = body;
        }

        #endregion ctor

        #region util

        /// <summary>
        /// 获取消息对象的类型
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static string GetContentType(Type contentType)
        {
            if (contentType == null)
                return string.Empty;

            return Regex.Replace(contentType.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 获取消息对象的类型
        /// </summary>
        /// <param name="content">Type of the content.</param>
        /// <returns></returns>
        public static string GetContentType(object content)
        {
            if (content == null)
                return string.Empty;

            return GetContentType(content.GetType());
        }

        #endregion util

        #region json

        /// <summary>
        /// 使用Json
        /// </summary>
        /// <param name="object">消息对象</param>
        /// <returns></returns>
        public static MessagePacket UseJson<T>(T @object)
        {
            return UseJson<T>(@object, Never.Serialization.SerializeEnvironment.JsonSerializer);
        }

        /// <summary>
        /// 使用Json
        /// </summary>
        /// <param name="object">消息对象</param>
        /// <param name="jsonSerilizer">json序列化对象，可以使用 Never.JsonNet.JsonNetSerializer对象 </param>
        /// <returns></returns>
        public static MessagePacket UseJson<T>(T @object, Serialization.IJsonSerializer jsonSerilizer)
        {
            if (@object == null)
                return new MessagePacket();

            var type = @object.GetType();
            return new MessagePacket()
            {
                ContentType = GetContentType(type),
                Body = jsonSerilizer.Serialize(@object),
            };
        }

        /// <summary>
        /// 从Json中获取对象
        /// </summary>
        /// <param name="packet">消息报文</param>
        /// <returns></returns>
        public static T FromJson<T>(MessagePacket packet)
        {
            return FromJson<T>(packet, Never.Serialization.SerializeEnvironment.JsonSerializer);
        }

        /// <summary>
        /// 从Json中获取对象
        /// </summary>
        /// <param name="packet">消息报文</param>
        /// <param name="jsonSerilizer">json序列化对象，可以使用 Never.JsonNet.JsonNetSerializer对象 </param>
        /// <returns></returns>
        public static T FromJson<T>(MessagePacket packet, Never.Serialization.IJsonSerializer jsonSerilizer)
        {
            if (packet == null)
                return default(T);

            if (string.IsNullOrEmpty(packet.ContentType) || string.IsNullOrEmpty(packet.Body))
                return default(T);

            var type = Type.GetType(packet.ContentType);
            if (type == null)
                return default(T);

            return jsonSerilizer.Deserialize<T>(packet.Body);
        }

        #endregion json
    }
}