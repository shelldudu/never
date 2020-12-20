using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Never.EasySql.Client
{
    /// <summary>
    /// access数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\test.mdb;uid=xx;pwd=xxx;")]
    public sealed class AccessExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter, IPageParameterHandler
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
        [Never.Attributes.Summary(Descn = "请先引用初始化Never.EasySql.SqlExecuterFactory.OleDbServerExecuter.DbProviderFactory")]
        public AccessExecuter(string connectionString)
            : base("@", DbProviderFactoryInstance ?? (DbProviderFactoryInstance = Never.EasySql.SqlExecuterFactory.OleDbServerExecuter.InitInstance()), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public AccessExecuter(DbProviderFactory provider, string connectionString)
            : base("@", provider, connectionString)
        {
        }

        /// <summary>
        /// 返回新的startIndex
        /// </summary>
        /// <param name="startIndex"></param>
        public int HandleStartIndex(int startIndex)
        {
            return startIndex <= 0 ? 1 : startIndex;
        }

        /// <summary>
        /// 返回新的endIndex
        /// </summary>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public int HandleEndIndex(int endIndex)
        {
            return endIndex <= 0 ? 1 : endIndex;
        }

        #endregion ctor
    }
}