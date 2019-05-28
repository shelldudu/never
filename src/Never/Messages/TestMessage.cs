using System;

namespace Never.Messages
{
    /// <summary>
    /// 测试消息
    /// </summary>
    [Serializable]
    public sealed class TestMessage : Never.Messages.IMessage
    {
        /// <summary>
        /// 测试消息Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 测试消息Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 测试消息其他
        /// </summary>
        public string Other { get; set; }
    }
}