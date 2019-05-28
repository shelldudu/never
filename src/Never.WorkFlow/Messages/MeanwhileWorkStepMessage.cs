using System;
using System.Collections.Generic;

namespace Never.WorkFlow.Messages
{
    /// <summary>
    /// 多个消息内容的工作流消息，不要返回该实例
    /// </summary>
    [Serializable]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class MeanwhileWorkStepMessage : IWorkStepMessage
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 附加状态
        /// </summary>
        public int AttachState { get; set; }

        /// <summary>
        /// 消息集合
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        ///
        /// </summary>
        public MeanwhileWorkStepMessage()
        {
            this.Messages = new List<string>(5);
        }
    }
}