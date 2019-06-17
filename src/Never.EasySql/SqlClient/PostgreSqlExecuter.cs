using Never.SqlClient;
using System.Data.Common;

namespace Never.EasySql.SqlClient
{
    /// <summary>
    /// PostgreSql 数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用初始化Never.SqlClient.SqlExecuterFactory.PostgreSqlExecuter.DbProviderFactory")]
    public sealed class PostgreSqlExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public PostgreSqlExecuter(string connectionString)
            : base(":", Never.SqlClient.SqlExecuterFactory.PostgreSqlExecuter.GetInstance(), connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public PostgreSqlExecuter(DbProviderFactory provider, string connectionString)
            : base(":", provider, connectionString)
        {
        }

        #endregion ctor
    }
}