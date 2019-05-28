using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// Call类型
    /// </summary>
    [Flags]
    public enum CallMode
    {
        /// <summary>
        /// 执行ExecuteScalar方法
        /// </summary>
        ExecuteScalar = 1,

        /// <summary>
        /// 执行ExecuteNonQuery方法
        /// </summary>
        ExecuteNonQuery = 2,

        /// <summary>
        /// 执行方法的命令类型
        /// </summary>
        CommandText = 4,

        /// <summary>
        /// 执行方法的命令类型
        /// </summary>
        CommandStoredProcedure = 8,

        /// <summary>
        /// 执行方法的命令类型
        /// </summary>
        CommandTableDirect = 16
    }
}
