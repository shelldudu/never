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
    public class DefaultSession : ISession
    {
        /// <summary>
        /// 数据库相关源
        /// </summary>
        public IDataSource DataSource { get; internal set; }

        /// <summary>
        /// 事务
        /// </summary>
        public IDbTransaction Transaction { get; internal set; }

        /// <summary>
        /// 数据操作接口
        /// </summary>
        public IDao Dao { get; internal set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.Transaction != null)
                this.Transaction.Dispose();

            if (this.Dao != null)
                this.Dao.Dispose();
        }
    }
}