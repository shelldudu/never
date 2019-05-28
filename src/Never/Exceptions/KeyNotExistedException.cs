using System;
using System.Runtime.Serialization;

namespace Never.Exceptions
{
    /// <summary>
    /// 值不存在异常
    /// </summary>
    public class KeyNotExistedException : InvalidException
    {
        #region field

        /// <summary>
        /// 出错key名称
        /// </summary>
        private readonly string key;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotExistedException"/> class.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="message">描述错误的消息</param>
        public KeyNotExistedException(string key, string message)
            : base(message)
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotExistedException"/> class.
        /// </summary>
        /// <param name="format">格式化字符.</param>
        /// <param name="key">key</param>
        /// <param name="paras">参数.</param>
        public KeyNotExistedException(string key, string format, params string[] paras)
            : base(string.Format(format, paras))
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotExistedException"/> class.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public KeyNotExistedException(string key, string message, Exception innerException)
            : base(message, innerException)
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotExistedException"/> class.
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected KeyNotExistedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion ctor

        #region Property

        /// <summary>
        /// 出错的参数名称
        /// </summary>
        public string Key
        {
            get
            {
                return this.key;
            }
        }

        #endregion Property
    }
}