using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 所有sqlTag提供者
    /// </summary>
    public interface ISqlTagProvider
    {
        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        SqlTag Get(string sqlId);

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        bool TryGet(string sqlId, out SqlTag sqlTag);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, SqlTag>> GetAll();
    }
}
