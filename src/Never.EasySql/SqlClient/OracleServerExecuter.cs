﻿using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Never.EasySql.SqlClient
{
    /// <summary>
    /// oracle 数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.OracleServerExecuter.DbProviderFactory")]
    public sealed class OracleServerExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleServerExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public OracleServerExecuter(string connectionString)
            : base(":", Never.SqlClient.OracleServerExecuter.GetInstance(), connectionString)
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
        }

        #endregion ctor
    }
}