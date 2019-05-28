using Never.Messages;
using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 领域命令消息
    /// </summary>
    [Serializable]
    public sealed class CommandStreamMessage : IMessage
    {
        /// <summary>
        /// 命令所在的运行域
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// 操作命令
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 命令名字
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        public string AggregateIdType { get; set; }

        /// <summary>
        /// 命令内容
        /// </summary>
        public string CommandContent { get; set; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        public int HashCode { get; set; }

        /// <summary>
        /// 当前环境的自增Id
        /// </summary>
        public long Increment { get; set; }
    }
}