using System;

namespace Never.Deployment
{
    /// <summary>
    /// api uri 健康信息
    /// </summary>
    public interface IApiUriHealthElement
    {
        #region prop属性

        /// <summary>
        /// 加入Id
        /// </summary>
        int Id { get;  }

        /// <summary>
        /// uri 地址
        /// </summary>
        string ApiUri { get; }

        /// <summary>
        /// 上一次更新时间
        /// </summary>
        DateTime ReportTime { get; }

        /// <summary>
        /// a10 地址
        /// </summary>
        string A10Url { get; }

        /// <summary>
        /// 上一次获取到的内容
        /// </summary>
        string ReportText { get; }

        /// <summary>
        /// 所属组
        /// </summary>
        string Group { get; }

        #endregion prop属性
    }
}