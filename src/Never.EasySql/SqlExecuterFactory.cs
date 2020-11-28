using Never.SqlClient;
using System;
using System.Data.Common;

namespace Never.EasySql
{
    /// <summary>
    /// sql工厂
    /// </summary>
    public static class SqlExecuterFactory
    {
        #region netsted

        /// <summary>
        /// mySql数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用MySql.Data组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class MySqlExecuter : Never.SqlClient.MySqlExecuter, ISqlExecuter, ITransactionExecuter
        {
            #region feild

            /// <summary>
            /// 工厂实例
            /// </summary>
            public static DbProviderFactory DbProviderFactoryInstance { get; set; }

            #endregion feild

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="MySqlExecuter"/> class.
            /// </summary>
            /// <param name="connectionString">连接字符串.</param>
            public MySqlExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(MySqlExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用MySql.Data组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "MySql.Data")
                        {
                            type = assembly.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        /// <summary>
        /// sqlserver数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.SqlClient组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class SqlServerExecuter : Never.SqlClient.SqlServerExecuter, ISqlExecuter, ITransactionExecuter
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
            public SqlServerExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(SqlServerExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用System.Data组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("System.Data.SqlClient.SqlClientFactory,System.Data");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "System.Data")
                        {
                            type = assembly.GetType("System.Data.SqlClient.SqlClientFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        /// <summary>
        /// odbcserver数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.Odbc组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class OdbcServerExecuter : Never.SqlClient.OdbcServerExecuter, ISqlExecuter, ITransactionExecuter
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
            public OdbcServerExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(OdbcServerExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用System.Data组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("System.Data.Odbc.OdbcFactory,System.Data");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "System.Data")
                        {
                            type = assembly.GetType("System.Data.Odbc.OdbcFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        /// <summary>
        /// oracle数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.OleDb组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class OleDbServerExecuter : Never.SqlClient.OleDbServerExecuter, ISqlExecuter, ITransactionExecuter
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
            public OleDbServerExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(OleDbServerExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用System.Data组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("System.Data.OleDb.OleDbFactory,System.Data");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "System.Data")
                        {
                            type = assembly.GetType("System.Data.OleDb.OleDbFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build

            #region prefix

            /// <summary>
            /// 获取Sql参数的前缀
            /// </summary>
            /// <returns></returns>
            public override string GetParameterPrefix()
            {
                return "@";
            }

            #endregion prefix

            #region dispose

            /// <summary>
            /// 释放连接对象
            /// </summary>
            /// <param name="isdispose">是否释放</param>
            protected override void Dispose(bool isdispose)
            {
                base.Dispose(isdispose);
            }

            #endregion dispose
        }

        /// <summary>
        /// oracle 数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用Oracle.ManagedDataAccess组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class OracleServerExecuter : Never.SqlClient.OracleServerExecuter, ISqlExecuter, ITransactionExecuter
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
            public OracleServerExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(OracleServerExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用Oracle.ManagedDataAccess组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "Oracle.ManagedDataAccess")
                        {
                            type = assembly.GetType("Oracle.ManagedDataAccess.Client.OracleClientFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用Npgsql组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class PostgreSqlExecuter : Never.SqlClient.PostgreSqlExecuter, ISqlExecuter, ITransactionExecuter
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
            public PostgreSqlExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(PostgreSqlExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用Npgsql组件或者初始化Instance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
                var type = Type.GetType("Npgsql.NpgsqlFactory,Npgsql");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "Npgsql")
                        {
                            type = assembly.GetType("Npgsql.NpgsqlFactory");
                            break;
                        }
                    }
                }

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        /// <summary>
        /// sqlserver数据库
        /// </summary>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.SQLite.SQLiteFactory或Microsoft.Data.Sqlite.SqliteFactory组件或者初始化DbProviderFactoryInstance属性对象")]
        public sealed class SqliteExecuter : Never.SqlClient.SqliteExecuter, ISqlExecuter, ITransactionExecuter
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
            public SqliteExecuter(string connectionString)
                : base(GetInstance(), connectionString)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 获取实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory GetInstance()
            {
                if (DbProviderFactoryInstance != null)
                    return DbProviderFactoryInstance;

                lock (typeof(SqliteExecuter))
                {
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;

                    DbProviderFactoryInstance = InitInstance();
                    if (DbProviderFactoryInstance != null)
                        return DbProviderFactoryInstance;
                }

                throw new TypeLoadException("请先引用System.Data.SQLite.SQLiteFactory或Microsoft.Data.Sqlite.SqliteFactory组件或者初始化DbProviderFactoryInstance属性对象");
            }

            /// <summary>
            /// 查询实例
            /// </summary>
            /// <returns></returns>
            public static DbProviderFactory InitInstance()
            {
#if !NET461
            var type = Type.GetType("Microsoft.Data.Sqlite.SqliteFactory,Microsoft.Data.Sqlite");
            if (type == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == "Microsoft.Data.Sqlite")
                    {
                        type = assembly.GetType("Microsoft.Data.Sqlite.SqliteFactory");
                        break;
                    }
                }
            }
#else
                var type = Type.GetType("System.Data.SQLite.SQLiteFactory,System.Data.SQLite");
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "System.Data.SQLite")
                        {
                            type = assembly.GetType("System.Data.SQLite.SQLiteFactory");
                            break;
                        }
                    }
                }
#endif

                return type == null ? null : CreateDbProviderFactory(type);
            }

            #endregion build
        }

        #endregion

        #region create

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用MySql.Data组件")]
        public static Never.SqlClient.MySqlExecuter MySql(string connectionString)
        {
            return new MySqlExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.SqlClient组件")]
        public static Never.SqlClient.SqlServerExecuter SqlServer(string connectionString)
        {
            return new SqlServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.Odbc组件")]
        public static Never.SqlClient.OdbcServerExecuter OdbcServer(string connectionString)
        {
            return new OdbcServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.OleDb组件")]
        public static Never.SqlClient.OleDbServerExecuter OleDbServer(string connectionString)
        {
            return new OleDbServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用Oracle.ManagedDataAccess组件")]
        public static Never.SqlClient.OracleServerExecuter OracleServer(string connectionString)
        {
            return new OracleServerExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用Npgsql组件")]
        public static Never.SqlClient.PostgreSqlExecuter PostgreSql(string connectionString)
        {
            return new PostgreSqlExecuter(connectionString);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "请先引用System.Data.SQLite.SQLiteFactory或Microsoft.Data.Sqlite.SqliteFactory组件")]
        public static Never.SqlClient.SqliteExecuter Sqlite(string connectionString)
        {
            return new SqliteExecuter(connectionString);
        }

        #endregion
    }
}