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
    /// odbcserver数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.OdbcServerExecuter.DbProviderFactory")]
    public sealed class OdbcServerExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public OdbcServerExecuter(string connectionString)
            : base("@", Never.SqlClient.OdbcServerExecuter.GetInstance(), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OdbcServerExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public OdbcServerExecuter(DbProviderFactory provider, string connectionString)
            : base("@", provider, connectionString)
        {
        }

        #endregion ctor
    }
}