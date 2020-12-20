using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 分页参数处理
    /// </summary>
    public interface IPageParameterHandler
    {
        /// <summary>
        /// 返回新的startIndex
        /// </summary>
        /// <param name="startIndex"></param>
        int HandleStartIndex(int startIndex);

        /// <summary>
        /// 返回新的endIndex
        /// </summary>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        int HandleEndIndex(int endIndex);
    }
}
