using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    sealed class LinqSqlTagProvider
    {
        /// <summary>
        /// 缓存起来的东西
        /// </summary>
        private static readonly SqlTagProvider provider = new SqlTagProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId">缓存key</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool Get(string cacheId, out LinqSqlTag tag)
        {
            tag = null;
            if (cacheId.IsNotNullOrWhiteSpace())
            {
                if (provider.TryGet(cacheId, out var temp))
                {
                    tag = (LinqSqlTag)temp;
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        public static LinqSqlTag Set(LinqSqlTag sqlTag)
        {
            provider.Add(sqlTag);
            return sqlTag;
        }
    }
}
