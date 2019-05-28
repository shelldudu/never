using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 系统繁忙异常，通常用于应用层与UI层的通信
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class SystemBusyException : Exception
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBusyException"/> class.
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public SystemBusyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBusyException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="paras">参数.</param>
        public SystemBusyException(string format, params string[] paras)
            : base(string.Format(format, paras))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBusyException"/> class.
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public SystemBusyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemBusyException"/> class.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" />，它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" />，它包含有关源或目标的上下文信息。</param>
        protected SystemBusyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor
    }
}