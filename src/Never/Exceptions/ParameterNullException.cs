using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 参数为空异常
    /// </summary>
    public class ParameterNullException : ParameterException
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNullException"/> class.
        /// </summary>
        /// <param name="parameterName">参数名称.</param>
        /// <param name="message">描述错误的消息.</param>
        public ParameterNullException(string parameterName, string message)
            : base(parameterName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNullException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="parameterName">key</param>
        /// <param name="paras">参数.</param>
        public ParameterNullException(string parameterName, string format, params string[] paras)
            : base(parameterName, string.Format(format, paras))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNullException"/> class.
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public ParameterNullException(string parameterName, string message, Exception innerException)
            : base(parameterName, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNullException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected ParameterNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor
    }
}