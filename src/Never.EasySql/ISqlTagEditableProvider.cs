using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// sqlTag可编译的提供者
    /// </summary>
    public interface ISqlTagEditableProvider : ISqlTagProvider
    {
        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        void Add(SqlTag sqlTag);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sqlTag"></param>
        void Remove(SqlTag sqlTag);
    }
}
