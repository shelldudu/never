using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 用于重试执行命令的异常
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class RepeatExcutingException : Exception
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatExcutingException"/> class.
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public RepeatExcutingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatExcutingException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="paras">参数.</param>
        public RepeatExcutingException(string format, params string[] paras)
            : base(string.Format(format, paras))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatExcutingException"/> class.
        /// </summary>
        /// <param name="message">解释异常原因的错误信息。</param>
        /// <param name="innerException">导致当前异常的异常。如果 <paramref name="innerException" /> 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常。</param>
        public RepeatExcutingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatExcutingException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected RepeatExcutingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor
    }
}