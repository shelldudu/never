using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// sql工厂
    /// </summary>
    public static class SqlExecuterFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用MySql.Data组件")]
        public static MySqlExecuter MySql(string connectionString)
        {
            return new MySqlExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static MySqlExecuter MySql(DbProviderFactory provider, string connectionString)
        {
            return new MySqlExecuter(provider, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlServerExecuter SqlServer(string connectionString)
        {
            return new SqlServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlServerExecuter SqlServer(DbProviderFactory provider, string connectionString)
        {
            return new SqlServerExecuter(provider, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OdbcServerExecuter OdbcServer(string connectionString)
        {
            return new OdbcServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OdbcServerExecuter OdbcServer(DbProviderFactory provider, string connectionString)
        {
            return new OdbcServerExecuter(provider, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OleDbServerExecuter OleDbServer(string connectionString)
        {
            return new OleDbServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OleDbServerExecuter OleDbServer(DbProviderFactory provider, string connectionString)
        {
            return new OleDbServerExecuter(provider, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用Oracle.ManagedDataAccess组件")]
        public static OracleServerExecuter OracleServer(string connectionString)
        {
            return new OracleServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static OracleServerExecuter OracleServer(DbProviderFactory provider, string connectionString)
        {
            return new OracleServerExecuter(provider, connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用Oracle.ManagedDataAccess组件")]
        public static PostgreSqlExecuter PostgreSql(string connectionString)
        {
            return new PostgreSqlExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用Npgsql组件或")]
        public static PostgreSqlExecuter PostgreSql(DbProviderFactory provider, string connectionString)
        {
            return new PostgreSqlExecuter(provider, connectionString);
        }
    }
}