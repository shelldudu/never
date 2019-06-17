using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Never.EasySql.SqlClient
{
    /// <summary>
    /// sqlite数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.SqlExecuterFactory.SqliteExecuter.DbProviderFactory")]
    public sealed class SqliteExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(string connectionString)
            : base("@", Never.SqlClient.SqlExecuterFactory.SqliteExecuter.GetInstance(), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(DbProviderFactory provider, string connectionString)
            : base("@", provider, connectionString)
        {
        }

        #endregion ctor
    }
}