using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Text;

namespace Never.EasySql.Client
{
    /// <summary>
    /// oracle数据库
    /// </summary>
    public sealed class OleDbServerExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region feild

        /// <summary>
        /// 工厂实例
        /// </summary>
        public static DbProviderFactory DbProviderFactoryInstance { get; set; }

        #endregion feild

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.SqlExecuterFactory.OleDbServerExecuter.DbProviderFactory")]
        public OleDbServerExecuter(string connectionString)
            : base("@", DbProviderFactoryInstance ?? (DbProviderFactoryInstance = Never.SqlClient.SqlExecuterFactory.OleDbServerExecuter.InitInstance()), connectionString)
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
            //cache the provider
            if (DbProviderFactoryInstance == null)
                DbProviderFactoryInstance = provider;
        }

        #endregion ctor
    }
}