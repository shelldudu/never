using System;
using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// sqlserver数据库
    /// </summary>
    [Never.Attributes.Summary(Descn = "请先引用System.Data.SQLite.SQLiteFactory或Microsoft.Data.Sqlite.SqliteFactory组件或者初始化DbProviderFactoryInstance属性对象")]
    public class SqliteExecuter : SqlExecuter, ISqlExecuter, ITransactionExecuter
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(DbProviderFactory provider, string connectionString)
            : base(provider, connectionString)
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
}