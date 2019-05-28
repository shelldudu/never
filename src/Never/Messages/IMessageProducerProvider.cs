using Never.Serialization;

namespace Never.Messages
{
    /// <summary>
    /// 消息提供者
    /// </summary>
    public interface IMessageProducerProvider
    {
        /// <summary>
        /// 查询Json序列化接口
        /// </summary>
        /// <returns></returns>
        IJsonSerializer JsonSerilizer { get; }

        /// <summary>
        /// 查询消息提供者
        /// </summary>
        /// <returns></returns>
        IMessageProducer MessageProducer { get; }
    }
}