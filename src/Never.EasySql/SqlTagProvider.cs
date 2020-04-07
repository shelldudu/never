using Never.Exceptions;
using Never.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// sqltag提供者
    /// </summary>
    /// <seealso cref="Never.EasySql.ISqlTagProvider" />
    public class SqlTagProvider : ISqlTagProvider, ISqlTagEditableProvider
    {
        #region field

        /// <summary>
        /// 所有入tag
        /// </summary>
        private readonly ConcurrentDictionary<string, Never.EasySql.SqlTag> sortedSet = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public SqlTagProvider()
        {
            this.sortedSet = new ConcurrentDictionary<string, Never.EasySql.SqlTag>(StringComparer.CurrentCulture);
        }

        #endregion ctor

        #region ISqlTagProvider

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, Never.EasySql.SqlTag>> GetAll()
        {
            return this.sortedSet;
        }

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public Never.EasySql.SqlTag Get(string sqlId)
        {
            TryGet(sqlId, out var sqlTag);
            return sqlTag;
        }

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        public bool TryGet(string sqlId, out Never.EasySql.SqlTag sqlTag)
        {
            if (!this.sortedSet.TryGetValue(sqlId, out sqlTag))
                return false;

            return true;
        }

        #endregion ISqlTagProvider

        #region load

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        public void Add(SqlTag sqlTag)
        {
            if (this.TryGet(sqlTag.Id, out var temp))
                return;

            this.sortedSet[sqlTag.Id] = sqlTag;
            return;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sqlTag"></param>
        public void Remove(SqlTag sqlTag)
        {
            this.sortedSet.TryRemove(sqlTag.Id, out var id);
        }

        #endregion load
    }
}