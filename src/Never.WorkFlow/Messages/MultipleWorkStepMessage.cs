using System;
using System.Collections.Generic;
using System.Linq;

namespace Never.WorkFlow.Messages
{
    /// <summary>
    /// 多个消息内容的工作流消息
    /// </summary>
    [Serializable]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class MultipleWorkStepMessage : IMultipleWorkStepMessage, IWorkStepMessage
    {
        #region prop

        /// <summary>
        /// 消息集合
        /// </summary>
        public IEnumerable<IWorkStepMessage> Messages { get; set; }

        /// <summary>
        /// 任务号，在创建了任务后，会有任务号
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 附加状态
        /// </summary>
        public int AttachState { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        private MultipleWorkStepMessage(IWorkStepMessage[] message)
        {
            this.Messages = message ?? new IWorkStepMessage[0];
        }

        /// <summary>
        ///
        /// </summary>
        public MultipleWorkStepMessage() : this(new IWorkStepMessage[0])
        {
        }

        #endregion ctor

        #region append

        /// <summary>
        /// 追加一个消息
        /// </summary>
        /// <param name="message"></param>
        public void Append(IWorkStepMessage message)
        {
            this.Messages = this.Messages.Union(new[] { message });
        }

        #endregion append

        #region empty

        /// <summary>
        /// 返回唯一实例
        /// </summary>
        public static MultipleWorkStepMessage Empty { get; } = new MultipleWorkStepMessage(new IWorkStepMessage[0]);

        #endregion empty
    }
}