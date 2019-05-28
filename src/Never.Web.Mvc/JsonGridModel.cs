using System.Collections;

namespace Never.Web.Mvc
{
    /// <summary>
    /// 返回Json对象
    /// </summary>
    public sealed class JsonGridModel
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public JsonGridModel()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="records"></param>
        public JsonGridModel(IEnumerable records)
        {
            this.Records = records;
        }

        #endregion ctor

        #region property

        /// <summary>
        /// 总计
        /// </summary>
        public object Aggregates { get; set; }

        /// <summary>
        /// 记录
        /// </summary>
        public IEnumerable Records { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageNow { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        #endregion property
    }
}