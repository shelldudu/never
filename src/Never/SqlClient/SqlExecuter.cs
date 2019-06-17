using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Never.SqlClient
{
    /// <summary>
    /// 默认的数据库读取接口，该对象实例安全，静态实例不安全
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class SqlExecuter : ISqlExecuter, IDisposable, IParameterPrefixProvider
    {
        #region prop

        /// <summary>
        /// 当前数据适配工厂
        /// </summary>
        private readonly DbProviderFactory factory = null;

        /// <summary>
        /// 数据库连接串
        /// </summary>
        private readonly string connectionString = null;

        #endregion prop

        #region field

        /// <summary>
        /// 是否使用事务
        /// </summary>
        public DbTransaction Transaction { get; private set; }

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExecuter"/> class.
        /// 构造函数,以connectionString链接名,providerName为数据工厂模式来创建对象实体
        /// </summary>
        /// <param name="factory">数据工厂.</param>
        /// <param name="connectionString">连接字符串.</param>
        protected SqlExecuter(DbProviderFactory factory, string connectionString)
        {
            if (factory == null)
                throw new ArgumentNullException("factory为空");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString为空");

            this.factory = factory;
            this.connectionString = connectionString;
        }

        #endregion ctor

        #region connection

        /// <summary>
        /// 当前数据适配工厂
        /// </summary>
        public DbProviderFactory ProviderFactory { get { return this.factory; } }

        /// <summary>
        /// 使用Type来构造<see cref="DbProviderFactory"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected static DbProviderFactory CreateDbProviderFactory(Type type)
        {
            var emit = EasyEmitBuilder<Func<DbProviderFactory>>.NewDynamicMethod();
            emit.NewObject(type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0]);
            emit.Return();
            return emit.CreateDelegate().Invoke();
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get { return this.connectionString; } }

        #endregion connection

        #region prefix

        /// <summary>
        /// 获取Sql参数的前缀
        /// </summary>
        /// <returns></returns>
        public abstract string GetParameterPrefix();

        #endregion prefix

        #region para and builder

        /// <summary>
        /// 创建数据库查询参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public DbParameter CreateParameter(string parameterName, object value)
        {
            var pa = this.factory.CreateParameter();
            pa.ParameterName = parameterName;
            pa.Value = value;

            return pa;
        }

        /// <summary>
        /// 创建数据库查询参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="value">对象的值</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public DbParameter CreateParameter(string parameterName, DbType dbType, object value)
        {
            var pa = this.factory.CreateParameter();
            pa.ParameterName = parameterName;
            pa.DbType = dbType;
            pa.Value = value;

            return pa;
        }

        /// <summary>
        /// 创建数据库查询参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="direction">输出输入参数类型</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public DbParameter CreateParameter(string parameterName, ParameterDirection direction, DbType dbType, object value)
        {
            var pa = this.factory.CreateParameter();
            pa.ParameterName = parameterName;
            pa.Direction = direction;
            pa.DbType = dbType;
            pa.Value = value;

            return pa;
        }

        /// <summary>
        /// 准备参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected IEnumerable<IDbDataParameter> ReadyParameters(KeyValuePair<string, object>[] @parameters)
        {
            var list = new List<IDbDataParameter>(@parameters.Length);
            foreach (var para in @parameters)
            {
                if (para.Value == null || para.Value == DBNull.Value)
                    list.Add(this.CreateParameter(para.Key, DBNull.Value));
                else
                    list.Add(this.CreateParameter(para.Key, para.Value));
            }

            return list;
        }

        /// <summary>
        /// 获取一个对参数进行sql语句构造者
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SqlParamerBuilder NewSqlParamerBuilder(string sql, object @parameter)
        {
            return new SqlParamerBuilder(sql).Build(this.GetParameterPrefix(), @parameter);
        }

        /// <summary>
        /// 获取一个对参数进行sql语句构造者
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlParamerBuilder NewSqlParamerBuilder(string sql, KeyValuePair<string, object>[] @parameters)
        {
            return new SqlParamerBuilder(sql).Build(this.GetParameterPrefix(), @parameters);
        }

        #endregion para and builder

        #region command

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <returns></returns>
        protected virtual DbConnection CreateDbConnection()
        {
            if (this.Transaction != null)
                return this.Transaction.Connection;

            var dbconnection = this.factory.CreateConnection();
            dbconnection.ConnectionString = this.connectionString;
            return dbconnection;
        }

        /// <summary>
        /// 创建command对象
        /// </summary>
        /// <param name="sql">查询字符串.</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameters">查询参数.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual DbCommand CreateDbCommand(string sql, CommandType commandType, IEnumerable<IDbDataParameter> parameters)
        {
            var cmd = this.factory.CreateCommand();
            cmd.Connection = this.CreateDbConnection();
            cmd.Transaction = this.Transaction;
            cmd.CommandType = commandType;
            cmd.CommandText = sql;
            foreach (var a in parameters)
            {
                if (a == null)
                    continue;

                cmd.Parameters.Add(a);
            }

            return cmd;
        }

        #endregion command

        #region trans

        /// <summary>
        /// 开启新事务
        /// </summary>
        public virtual void BeginTransaction()
        {
            this.BeginTransaction(IsolationLevel.Chaos);
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        public virtual void BeginTransaction(IsolationLevel level)
        {
            if (this.Transaction != null)
                return;

            var dbconnection = this.CreateDbConnection();
            if (dbconnection.State != ConnectionState.Open)
                dbconnection.Open();

            this.Transaction = dbconnection.BeginTransaction(level);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void CommitTransaction()
        {
            this.CommitTransaction(true);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public virtual void CommitTransaction(bool closeConnection)
        {
            if (this.Transaction == null)
                throw new System.Data.DataException("the transaction is null");

            if (this.Transaction.Connection == null)
                throw new System.Data.DataException("the connection is null");

            if (this.Transaction.Connection.State == ConnectionState.Closed)
                return;

            this.Transaction.Commit();
            if (closeConnection && this.Transaction != null && this.Transaction.Connection != null && this.Transaction.Connection.State != ConnectionState.Closed)
                this.Transaction.Connection.Close();

            this.Transaction.Dispose();
            this.Transaction = null;
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackTransaction()
        {
            this.RollBackTransaction(true);
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public virtual void RollBackTransaction(bool closeConnection)
        {
            if (this.Transaction == null)
                throw new System.Data.DataException("the transaction is null");

            if (this.Transaction.Connection == null)
                throw new System.Data.DataException("the connection is null");

            this.Transaction.Rollback();
            if (closeConnection && this.Transaction != null && this.Transaction.Connection != null && this.Transaction.Connection.State != ConnectionState.Closed)
                this.Transaction.Connection.Close();

            this.Transaction.Dispose();
            this.Transaction = null;
        }

        #endregion trans

        #region dataset

        /// <summary>
        /// 返回DataSet,性能较低，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataSet CreateDataSet(string sql, object @parameter)
        {
            return this.CreateDataSet(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回DataSet,性能较低
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataSet CreateDataSet(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.CreateDataSet(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.CreateDataSet(cmd);
            }
        }

        /// <summary>
        /// 返回DataSet,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected DataSet CreateDataSet(IDbCommand command)
        {
            return this.CreateDataSet(command, true);
        }

        /// <summary>
        /// 返回DataSet,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual DataSet CreateDataSet(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            var ds = default(DataSet);
            IDbDataAdapter adapter = this.factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            try
            {
                ds = new DataSet("data");
                adapter.Fill(ds);
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
        }

        #endregion dataset

        #region datatable

        /// <summary>
        /// 获取DataTable,性能较低，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataTable CreateTable(string sql, object @parameter)
        {
            return this.CreateTable(sql, System.Data.CommandType.Text, @parameter);
        }

        /// <summary>
        /// 获取DataTable,性能较低
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataTable CreateTable(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.CreateTable(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.CreateTable(cmd);
            }
        }

        /// <summary>
        /// 获取DataTable,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected DataTable CreateTable(IDbCommand command)
        {
            return this.CreateTable(command, true);
        }

        /// <summary>
        /// 获取DataTable,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual DataTable CreateTable(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            var table = new System.Data.DataTable("data");
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));

                    while (reader.Read())
                    {
                        var row = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                            row[i] = reader[i];

                        table.Rows.Add(row);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }

            return table;
        }

        #endregion datatable

        #region dataview

        /// <summary>
        /// 获取DataView,性能较低，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataView CreateDataView(string sql, object @parameter)
        {
            return this.CreateDataView(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 获取DataView,性能较低
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public DataView CreateDataView(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.CreateDataView(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.CreateDataView(cmd);
            }
        }

        /// <summary>
        /// 获取DataView,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected DataView CreateDataView(IDbCommand command)
        {
            return this.CreateDataView(command, true);
        }

        /// <summary>
        /// 获取DataView,性能较低
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual DataView CreateDataView(IDbCommand command, bool closeConnection)
        {
            var table = this.CreateTable(command, closeConnection);
            if (table != null)
                return table.DefaultView;

            return new DataView();
        }

        #endregion dataview

        #region datarow

        /// <summary>
        /// 返回DataRow一行，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IDictionary CreateSingleRow(string sql, object @parameter)
        {
            return this.CreateSingleRow(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回DataRow一行
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式.</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IDictionary CreateSingleRow(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.CreateSingleRow(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.CreateSingleRow(cmd);
            }
        }

        /// <summary>
        /// 返回DataRow一行
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected IDictionary CreateSingleRow(IDbCommand command)
        {
            return this.CreateSingleRow(command, true);
        }

        /// <summary>
        /// 返回DataRow一行
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual IDictionary CreateSingleRow(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            var dic = default(IDictionary);
            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            try
            {
                using (var dr = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (dr.Read())
                    {
                        dic = new Dictionary<string, object>(dr.FieldCount);
                        for (int i = 0; i < dr.FieldCount; i++)
                            dic.Add(dr.GetName(i), dr.GetValue(i));
                    }
                }
                return dic;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
        }

        #endregion datarow

        #region execute nonquery

        /// <summary>
        /// 返回执行影响行数，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object @parameter)
        {
            return this.ExecuteNonQuery(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.ExecuteNonQuery(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected int ExecuteNonQuery(IDbCommand command)
        {
            return this.ExecuteNonQuery(command, true);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual int ExecuteNonQuery(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            try
            {
                return command.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
        }

        #endregion execute nonquery

        #region execute scalar

        /// <summary>
        /// 返回执行第一行第一列的值，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object @parameter)
        {
            return this.ExecuteScalar(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.ExecuteScalar(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected object ExecuteScalar(IDbCommand command)
        {
            return this.ExecuteScalar(command, true);
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual object ExecuteScalar(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            try
            {
                return command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
        }

        #endregion execute scalar

        #region reader

        /// <summary>
        /// 获取DbDataReader,高速度，推荐使用，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IDataReader CreateReader(string sql, object @parameter)
        {
            return this.CreateReader(sql, System.Data.CommandType.Text, @parameter);
        }

        /// <summary>
        /// 获取DbDataReader
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IDataReader CreateReader(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.CreateReader(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.CreateReader(cmd);
            }
        }

        /// <summary>
        /// 获取DbDataReader
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <param name="checkParameterMatched">是否检查参数的匹配性，如果为true,则要sql中所需要的参数都要在参数提供者中找到</param>
        /// <returns></returns>
        protected IDataReader CreateReader(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter, bool checkParameterMatched)
        {
            if (checkParameterMatched)
            {
                var builder = this.NewSqlParamerBuilder(sql, parameter);
                if (this.Transaction != null)
                    return this.CreateReader(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

                using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
                {
                    return this.CreateReader(cmd);
                }
            }

            if (this.Transaction != null)
                return this.CreateReader(this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)));

            using (var cmd = this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)))
            {
                return this.CreateReader(cmd);
            }
        }

        /// <summary>
        /// 获取DbDataReader,高速度，推荐使用
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected IDataReader CreateReader(IDbCommand command)
        {
            return this.CreateReader(command, true);
        }

        /// <summary>
        /// 获取DbDataReader,高速度，推荐使用
        /// </summary>
        /// <param name="command">创建数据库操作命令对象</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual IDataReader CreateReader(IDbCommand command, bool closeConnection)
        {
            if (command == null || (command != null && command.Connection == null))
                throw new ArgumentNullException("构造Connection对象为空");

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            return closeConnection ? command.ExecuteReader(CommandBehavior.CloseConnection) : command.ExecuteReader();
        }

        /// <summary>
        /// 查询列表，没有对阻抗失败的做异常处理
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数,使用匿名类</param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string sql, object @parameter)
        {
            return this.QueryForEnumerable<T>(sql, System.Data.CommandType.Text, @parameter);
        }

        /// <summary>
        /// 查询列表，没有对阻抗失败的做异常处理
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.QueryForEnumerable<T>(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.QueryForEnumerable<T>(cmd);
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        protected IEnumerable<T> QueryForEnumerable<T>(IDbCommand command)
        {
            return this.QueryForEnumerable<T>(command, true);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="command">查询命令</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> QueryForEnumerable<T>(IDbCommand command, bool closeConnection)
        {
            var @delegate = DataRecordBuilder<T>.Func;
            var result = new List<T>();
            IDataReader reader = null;
            try
            {
                using (reader = this.CreateReader(command))
                {
                    var rd = new IDataRecordDecorator(reader);
                    while (reader.Read())
                    {
                        var value = @delegate(rd.Load(reader));
                        result.Add(value);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql, object @parameter)
        {
            return this.QueryForObject<T>(sql, System.Data.CommandType.Text, @parameter);
        }

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql, CommandType commandType, object @parameter)
        {
            var builder = this.NewSqlParamerBuilder(sql, parameter);
            if (this.Transaction != null)
                return this.QueryForObject<T>(this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)));

            using (var cmd = this.CreateDbCommand(builder.ToString(), commandType, this.ReadyParameters(builder.Paramters)))
            {
                return this.QueryForObject<T>(cmd);
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="command">查询命令</param>
        /// <returns></returns>
        protected T QueryForObject<T>(IDbCommand command)
        {
            return this.QueryForObject<T>(command, true);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="command">查询命令</param>
        /// <param name="closeConnection">关闭数据库连接</param>
        /// <returns></returns>
        protected virtual T QueryForObject<T>(IDbCommand command, bool closeConnection)
        {
            var @delegate = DataRecordBuilder<T>.Func;
            IDataReader reader = null;
            try
            {
                using (reader = this.CreateReader(command))
                {
                    var rd = new IDataRecordDecorator(reader);
                    if (reader.Read())
                    {
                        return @delegate(rd.Load(reader));
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.Transaction == null && closeConnection && reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return default(T);
        }

        #endregion reader

        #region insert

        /// <summary>
        /// 返回执行第一行第一列的值，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public object Insert(string sql, object @parameter)
        {
            return this.Insert(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public object Insert(string sql, CommandType commandType, object @parameter)
        {
            /*要检查是否以insert into 开头的*/
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\binsert\s+into\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("insert 语句请使用insert into语法");
            }

            return this.ExecuteScalar(sql, commandType, @parameter);
        }

        #endregion insert

        #region update

        /// <summary>
        /// 返回执行影响行数，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int Update(string sql, object @parameter)
        {
            return this.Update(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int Update(string sql, CommandType commandType, object @parameter)
        {
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\bupdate\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("update 语句请使用update语法");
            }

            return this.ExecuteNonQuery(sql, commandType, @parameter);
        }

        #endregion update

        #region delete

        /// <summary>
        /// 返回执行影响行数，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int Delete(string sql, object @parameter)
        {
            return this.Delete(sql, CommandType.Text, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int Delete(string sql, CommandType commandType, object @parameter)
        {
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\bdelete\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("delete 语句请使用delete语法");
            }

            return this.ExecuteNonQuery(sql, commandType, @parameter);
        }

        #endregion delete

        #region IDisposabel成员

        /// <summary>
        /// 释放连接对象
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放连接对象
        /// </summary>
        /// <param name="isdispose">是否释放</param>
        protected virtual void Dispose(bool isdispose)
        {
            if (!isdispose)
                return;

            /*直接放弃连接对象，即DbConn.Open()在同一段代码不可再用*/
            this.Transaction = null;
        }

        #endregion IDisposabel成员
    }
}