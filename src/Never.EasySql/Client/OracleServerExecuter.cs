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
    /// oracle 数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "data source=(description=(address_list=(address=(protocol=tcp)(host=127.0.0.11)(port=1521)))(connect_data=(service_name=test)));user id=xx;password=xxx;")]
    public sealed class OracleServerExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region feild

        /// <summary>
        /// 工厂实例
        /// </summary>
        public static DbProviderFactory DbProviderFactoryInstance { get; set; }

        #endregion feild

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleServerExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        [Never.Attributes.Summary(Descn = "请先引用初始化Never.EasySql.SqlClient.OracleServerExecuter.DbProviderFactory")]
        public OracleServerExecuter(string connectionString)
            : base(":", DbProviderFactoryInstance ?? (DbProviderFactoryInstance = Never.EasySql.SqlExecuterFactory.OracleServerExecuter.InitInstance()), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleServerExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public OracleServerExecuter(DbProviderFactory provider, string connectionString)
            : base(":", provider, connectionString)
        {
            //cache the provider
            if (DbProviderFactoryInstance == null)
                DbProviderFactoryInstance = provider;
        }

        #endregion ctor
    }
}