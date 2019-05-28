using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 工作流消息，如果消息要等待的，请返回<see cref="Messages.EmptyButWaitingWorkStepMessage"/>该类型
    /// </summary>
    public interface IWorkStepMessage : Never.Messages.IMessage
    {
        /// <summary>
        /// 任务号，具体对象最好设置set方法
        /// </summary>
        Guid TaskId { get; }

        /// <summary>
        /// 附加状态
        /// </summary>
        int AttachState { get; }
    }
}