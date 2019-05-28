using System.Collections.Generic;

namespace Never
{
    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>

    [System.Runtime.Serialization.DataContract]
    public class PagedData<TResult>
    {
        #region property

        /// <summary>
        /// 页索引
        /// </summary>

        [System.Runtime.Serialization.DataMember(Name = "PageNow")]
        [Serialization.Json.DataMember(Name = "PageNow")]
        public int PageNow { get; protected set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        [System.Runtime.Serialization.DataMember(Name = "PageSize")]
        [Serialization.Json.DataMember(Name = "PageSize")]
        public int PageSize { get; protected set; }

        /// <summary>
        /// 总记录数
        /// </summary>

        [System.Runtime.Serialization.DataMember(Name = "TotalCount")]
        [Serialization.Json.DataMember(Name = "TotalCount")]
        public int TotalCount { get; protected set; }

        /// <summary>
        /// 记录集合
        /// </summary>

        [System.Runtime.Serialization.DataMember(Name = "Records")]
        [Serialization.Json.DataMember(Name = "Records")]
        public IEnumerable<TResult> Records { get; protected set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedData{TResult}"/> class.
        /// 默认当前页为第一页，分页大小为15
        /// </summary>
        public PagedData()
            : this(1, 15, 0, new TResult[] { })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedData{TResult}"/> class.
        /// 该算法会从 records 重新分页获取数据
        /// </summary>
        /// <param name="pageNow">当前页.</param>
        /// <param name="pageSize">分页大小.</param>
        /// <param name="records">结果集.</param>
        public PagedData(int pageNow, int pageSize, IEnumerable<TResult> records)
        {
            this.PageSize = pageSize;
            this.PageNow = pageNow;
            if (records == null)
            {
                this.TotalCount = 0;
                this.Records = new TResult[0];
            }
            else
            {
                this.TotalCount = System.Linq.Enumerable.Count(records);
                if (this.TotalCount == 0)
                {
                    this.Records = new TResult[0];
                }
                else
                {
                    this.Records = System.Linq.Enumerable.Take(System.Linq.Enumerable.Skip(records, (pageNow - 1) * pageSize), pageSize < this.TotalCount ? pageSize : this.TotalCount);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedData{TResult}"/> class.
        /// </summary>
        /// <param name="pageNow">当前页.</param>
        /// <param name="pageSize">分页大小.</param>
        /// <param name="totalCount">总条数.</param>
        /// <param name="records">结果集.</param>
        public PagedData(int pageNow, int pageSize, int totalCount, IEnumerable<TResult> records)
        {
            this.PageSize = pageSize;
            this.PageNow = pageNow;
            this.TotalCount = totalCount;
            this.Records = records;
        }

        #endregion ctor
    }
}