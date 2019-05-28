using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 参数异常
    /// </summary>
    public class ParameterException : InvalidException
    {
        #region field

        /// <summary>
        /// 出错的参数名称
        /// </summary>
        private readonly string parameterName;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterException"/> class.
        /// </summary>
        /// <param name="parameterName">参数名称.</param>
        /// <param name="message">描述错误的消息.</param>
        public ParameterException(string parameterName, string message)
            : base(message)
        {
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="parameterName">key</param>
        /// <param name="paras">参数.</param>
        public ParameterException(string parameterName, string format, params string[] paras)
            : base(string.Format(format, paras))
        {
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterException"/> class.
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public ParameterException(string parameterName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected ParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor

        #region Property

        /// <summary>
        /// 出错的参数名称
        /// </summary>
        public virtual string ParameterName
        {
            get
            {
                return this.parameterName;
            }
        }

        #endregion Property
    }
}