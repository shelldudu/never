using Never.Attributes;

namespace Never
{
    /// <summary>
    /// 结果状态
    /// </summary>
    public enum ApiStatus
    {
        /// <summary>
        /// 错误
        /// </summary>
        [Remark(Name = "错误")]
        Error = -1,

        /// <summary>
        /// 失败
        /// </summary>
        [Remark(Name = "失败")]
        Fail = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Remark(Name = "成功")]
        Success = 1
    }
}