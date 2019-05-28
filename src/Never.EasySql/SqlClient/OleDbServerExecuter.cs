using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Text;

namespace Never.EasySql.SqlClient
{
    /// <summary>
    /// oracle数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.OleDbServerExecuter.DbProviderFactory")]
    public sealed class OleDbServerExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public OleDbServerExecuter(string connectionString)
            : base("@", Never.SqlClient.OleDbServerExecuter.GetInstance(), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbServerExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public OleDbServerExecuter(DbProviderFactory provider, string connectionString)
            : base("@", provider, connectionString)
        {
        }

        #endregion ctor
    }
}