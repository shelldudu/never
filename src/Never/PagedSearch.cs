using System;

namespace Never
{
    /// <summary>
    ///  分页搜索
    /// </summary>
    public class PagedSearch
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedSearch"/> class.
        /// </summary>
        public PagedSearch()
            : this(1, 15)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedSearch"/> class.
        /// </summary>
        /// <param name="pageNow">当前页.</param>
        /// <param name="pageSize">分页大小.</param>
        public PagedSearch(int pageNow, int pageSize)
        {
            this.PageNow = pageNow <= 1 ? 1 : pageNow;
            this.PageSize = pageSize <= 1 ? 1 : pageSize;
        }

        #endregion ctor

        #region property

        /// <summary>
        /// 索引开始
        /// </summary>
        public virtual int StartIndex
        {
            get
            {
                int index = 0;
                if (this.PageSize > 0 && this.PageNow > 0)
                    index = (this.PageNow - 1) * this.PageSize;

                return index;
            }
        }

        /// <summary>
        /// 索引结束
        /// </summary>
        public virtual int EndIndex
        {
            get
            {
                int index = 1;
                if (this.PageSize > 0 && this.PageNow > 0)
                    index = (this.PageNow * this.PageSize) - 1;

                return index;
            }
        }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 第几页数
        /// </summary>
        public int PageNow { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        #endregion property
    }
}