using System;
using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// oracle数据库
    /// </summary>
    public abstract class OleDbServerExecuter : SqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbServerExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public OleDbServerExecuter(DbProviderFactory provider, string connectionString)
            : base(provider, connectionString)
        {
        }

        #endregion ctor

        #region prefix

        /// <summary>
        /// 获取Sql参数的前缀
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return "@";
        }

        #endregion prefix

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