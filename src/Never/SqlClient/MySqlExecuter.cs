using System;
using System.Data;
using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// mySql数据库
    /// </summary>
    public abstract class MySqlExecuter : SqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        protected MySqlExecuter(DbProviderFactory provider, string connectionString)
            : base(provider, connectionString)
        {
        }

        #endregion ctor

        #region ovrride

        /// <summary>
        /// 获取Sql参数的前缀
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return "?";
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public override void BeginTransaction()
        {
            this.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        #endregion ovrride

        #region dispose

        /// <summary>
        /// 释放连接对象
        /// </summary>
        /// <param name="isdispose">是否释放</param>
        protected override void Dispose(bool isdispose)
        {
            base.Dispose(isdispose);
        }

        #endregion dispose
    }
}