using System.Collections.Generic;

namespace Never.WorkFlow.Messages
{
    /// <summary>
    /// 多个消息内容的工作流消息
    /// </summary>
    public interface IMultipleWorkStepMessage : IWorkStepMessage
    {
        /// <summary>
        /// 消息集合
        /// </summary>
        IEnumerable<IWorkStepMessage> Messages { get; }
    }
}