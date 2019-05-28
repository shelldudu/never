using System;

namespace Never.WorkFlow.Messages
{
    /// <summary>
    /// 空但并且是中断的消息
    /// </summary>
    [Serializable]
    public sealed class EmptyAndAbortedWorkStepMessage : IWorkStepMessage
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
        public string Message { get; set; }

        /// <summary>
        /// 附加状态
        /// </summary>
        public int AttachState { get; set; }
    }
}