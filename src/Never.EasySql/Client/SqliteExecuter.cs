using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Never.EasySql.Client
{
    /// <summary>
    /// sqlite数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\test.mdb;")]
    public sealed class SqliteExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region feild

        /// <summary>
        /// 工厂实例
        /// </summary>
        public static DbProviderFactory DbProviderFactoryInstance { get; set; }

        #endregion feild

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        [Never.Attributes.Summary(Descn = "请先引用初始化Never.EasySql.SqlClient.SqliteExecuter.DbProviderFactory")]
        public SqliteExecuter(string connectionString)
            : base("@", DbProviderFactoryInstance ?? (DbProviderFactoryInstance = Never.EasySql.SqlExecuterFactory.SqliteExecuter.InitInstance()), connectionString)
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
            //cache the provider
            if (DbProviderFactoryInstance == null)
                DbProviderFactoryInstance = provider;
        }

        #endregion ctor
    }
}