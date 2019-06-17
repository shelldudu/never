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
    /// mySql数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.SqlExecuterFactory.MySqlExecuter.DbProviderFactory")]
    public sealed class MySqlExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public MySqlExecuter(string connectionString)
            : base("?", Never.SqlClient.SqlExecuterFactory.MySqlExecuter.GetInstance(), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public MySqlExecuter(DbProviderFactory provider, string connectionString)
            : base("?", provider, connectionString)
        {
        }

        #endregion ctor
    }
}