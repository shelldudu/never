using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 执行session，只有开了事务才不为空
    /// </summary>
    public interface ISession : System.IDisposable
    {
        /// <summary>
        /// 数据库相关源
        /// </summary>
        IDataSource DataSource { get; }

        /// <summary>
        /// 事务
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// 数据操作接口
        /// </summary>
        IDao Dao { get; }
    }
}