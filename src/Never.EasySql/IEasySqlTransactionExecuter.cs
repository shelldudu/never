using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 事务执行者
    /// </summary>
    public interface IEasySqlTransactionExecuter
    {
        /// <summary>
        /// 开启新事务
        /// </summary>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        IDbTransaction BeginTransaction(IsolationLevel level);

        /// <summary>
        /// 提交
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        void CommitTransaction(bool closeConnection);

        /// <summary>
        /// 回滚
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        void RollBackTransaction(bool closeConnection);
    }
}