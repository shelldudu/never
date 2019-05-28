using System;

namespace Never.Deployment
{
    /// <summary>
    /// api uri 健康信息
    /// </summary>
    internal class ApiUriHealthElement : IApiUriHealthElement
    {
        #region prop属性

        /// <summary>
        /// 加入Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// uri 地址
        /// </summary>
        public string ApiUri { get; set; }

        /// <summary>
        /// 上一次更新时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// a10 地址
        /// </summary>
        public string A10Url { get; set; }

        /// <summary>
        /// 上一次获取到的内容
        /// </summary>
        public string ReportText { get; set; }

        /// <summary>
        /// 所属组
        /// </summary>
        public string Group { get; set; }

        #endregion prop属性

        #region ctor

        /// <summary>
        /// The identifier
        /// </summary>
        private static int _id = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUriHealthElement"/> class.
        /// </summary>
        public ApiUriHealthElement()
        {
            this.Id = System.Threading.Interlocked.Increment(ref _id);
        }

        #endregion
    }
}