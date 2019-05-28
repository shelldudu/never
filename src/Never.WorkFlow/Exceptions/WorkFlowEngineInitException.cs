using System;
using System.Runtime.Serialization;

namespace Never.WorkFlow.Exceptions
{
    /// <summary>
    /// 初始化工作流出错异常
    /// </summary>
    [Serializable]
    public class WorkFlowEngineInitException : Exception
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowEngineInitException"/> class.
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public WorkFlowEngineInitException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowEngineInitException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="paras">参数.</param>
        public WorkFlowEngineInitException(string format, params string[] paras)
            : base(string.Format(format, paras))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowEngineInitException"/> class.
        /// </summary>
        /// <param name="message">解释异常原因的错误信息。</param>
        /// <param name="innerException">导致当前异常的异常。如果 <paramref name="innerException" /> 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常。</param>
        public WorkFlowEngineInitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowEngineInitException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected WorkFlowEngineInitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor
    }
}