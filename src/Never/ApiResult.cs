using Never.Attributes;
using System;

namespace Never
{
    /// <summary>
    /// Api接口数据结果，通常用于对内接口
    /// </summary>
    /// <typeparam name="TResult">结果对象</typeparam>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public sealed class ApiResult<TResult>
    {
        #region property

        /// <summary>
        /// 代号，也是状态值
        /// </summary>
        [DefaultValue(Value = "ResultStatus.Fail")]
        [System.Runtime.Serialization.DataMember(Name = "status")]
        [Serialization.Json.DataMember(Name = "status")]
        public ApiStatus Status { get; set; }

        /// <summary>
        /// 附带信息
        /// </summary>
        [DefaultValue(Value = "string.Empty")]
        [System.Runtime.Serialization.DataMember(Name = "message")]
        [Serialization.Json.DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        [DefaultValue(Value = "default(TResult)")]
        [System.Runtime.Serialization.DataMember(Name = "result")]
        [Serialization.Json.DataMember(Name = "result")]
        public TResult Result { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        public ApiResult()
            : this(ApiStatus.Fail, default(TResult), string.Empty)
        {
            /*保留这个是因为在Wcf中用到契约的话，没这个构造则提示契约不可用*/
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="result">The result.</param>
        public ApiResult(ApiStatus status, TResult result)
            : this(status, result, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        public ApiResult(ApiStatus status, TResult result, string message)
        {
            this.Status = status;
            this.Result = result;
            this.Message = message;
        }

        #endregion ctor
    }
}