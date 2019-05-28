using System;

namespace Never.WorkFlow.Messages
{
    /// <summary>
    /// 执行异常的消息
    /// </summary>
    [Serializable]
    public sealed class ExecutingErrorWorkStepMessage : IWorkStepMessage
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 用户签名
        /// </summary>
        public string UserSign { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 中断消息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 队列消息
        /// </summary>
        public string PushMessage { get; set; }

        /// <summary>
        /// 附加状态
        /// </summary>
        public int AttachState { get; set; }
    }
}