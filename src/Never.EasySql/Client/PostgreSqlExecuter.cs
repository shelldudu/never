using Never.SqlClient;
using System.Data.Common;

namespace Never.EasySql.Client
{
    /// <summary>
    /// PostgreSql 数据库
    /// </summary> 
    [Never.Attributes.Summary(Descn = "server=127.0.0.1;uid=xx;pwd=xx;database=test;")]
    public sealed class PostgreSqlExecuter : EasySqlExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region feild

        /// <summary>
        /// 工厂实例
        /// </summary>
        public static DbProviderFactory DbProviderFactoryInstance { get; set; }

        #endregion feild

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        [Never.Attributes.Summary(Descn = "请先引用初始化Never.EasySql.SqlClient.PostgreSqlExecuter.DbProviderFactory")]
        public PostgreSqlExecuter(string connectionString)
            : base(":", DbProviderFactoryInstance ?? (DbProviderFactoryInstance = Never.EasySql.SqlExecuterFactory.PostgreSqlExecuter.InitInstance()), connectionString)
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
            //cache the provider
            if (DbProviderFactoryInstance == null)
                DbProviderFactoryInstance = provider;
        }

        #endregion ctor
    }
}