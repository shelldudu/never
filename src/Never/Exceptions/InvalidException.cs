﻿using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 有病的异常，如果用于领域对象中，通常表示不能对外显示的异常信息（与领域异常相对)
    /// </summary>
    public class InvalidException : FriendlyException
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidException"/> class.
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public InvalidException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="paras">参数.</param>
        public InvalidException(string format, params string[] paras)
            : base(string.Format(format, paras))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidException"/> class.
        /// </summary>
        /// <param name="message">解释异常原因的错误信息。</param>
        /// <param name="innerException">导致当前异常的异常。如果 <paramref name="innerException" /> 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常。</param>
        public InvalidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected InvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor
    }
}