using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    ///
    /// </summary>
    public class EasySqlExecuter : SqlExecuter, IEasySqlExecuter, IEasySqlTransactionExecuter, IParameterPrefixProvider
    {
        #region field

        private readonly string parameterPrefix = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EasySqlExecuter"/> class.
        /// 构造函数,以connectionString链接名,providerName为数据工厂模式来创建对象实体
        /// </summary>
        /// <param name="parameterPrefix">参数前缀</param>
        /// <param name="factory">数据工厂.</param>
        /// <param name="connectionString">连接字符串.</param>
        protected EasySqlExecuter(string parameterPrefix, DbProviderFactory factory, string connectionString) : base(factory, connectionString)
        {
            this.parameterPrefix = parameterPrefix;
        }

        #endregion ctor

        #region sqlExecuter

        /// <summary>
        /// 获取Sql参数的前缀
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return this.parameterPrefix;
        }

        #endregion sqlExecuter

        #region IEasySqlExecuter

        /// <summary>
        /// 获取DbDataReader，没有对阻抗失败的做异常处理
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (this.Transaction != null)
                return this.QueryForEnumerable<T>(this.CreateDbCommand(sql, commandType, this.ReadyParameters(parameter)));

            using (var cmd = this.CreateDbCommand(sql, commandType, this.ReadyParameters(parameter)))
            {
                return this.QueryForEnumerable<T>(cmd);
            }
        }

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (this.Transaction != null)
                return this.QueryForObject<T>(this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)));

            using (var cmd = this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)))
            {
                return this.QueryForObject<T>(cmd);
            }
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public virtual object Insert(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            /*要检查是否以insert into 开头的*/
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\binsert\s+into\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("insert 语句请使用insert into语法");
            }

            return this.ExecuteScalar(sql, commandType, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public virtual int Update(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\bupdate\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("update 语句请使用update语法");
            }

            return this.ExecuteNonQuery(sql, commandType, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public virtual int Delete(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (commandType == CommandType.Text)
            {
                if (!Regex.IsMatch(sql, @"\bdelete\s+", RegexOptions.IgnoreCase))
                    throw new ArgumentException("delete 语句请使用delete语法");
            }

            return this.ExecuteNonQuery(sql, commandType, @parameter);
        }

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (this.Transaction != null)
                return this.ExecuteNonQuery(this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)));

            using (var cmd = this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)))
            {
                return this.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数，目前支持<see cref="Hashtable"/>与匿名对象</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter)
        {
            if (this.Transaction != null)
                return this.ExecuteScalar(this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)));

            using (var cmd = this.CreateDbCommand(sql, commandType, this.ReadyParameters(@parameter)))
            {
                return this.ExecuteScalar(cmd);
            }
        }

        #endregion IEasySqlExecuter

        #region IEasySqlTransactionExecuter

        void ITransactionExecuter.BeginTransaction()
        {
            this.BeginTransaction();
        }

        void ITransactionExecuter.BeginTransaction(IsolationLevel level)
        {
            this.BeginTransaction(level);
        }

        void ITransactionExecuter.CommitTransaction()
        {
            this.CommitTransaction();
        }

        void ITransactionExecuter.CommitTransaction(bool closeConnection)
        {
            this.CommitTransaction(closeConnection);
        }

        void ITransactionExecuter.RollBackTransaction()
        {
            this.RollBackTransaction();
        }

        void ITransactionExecuter.RollBackTransaction(bool closeConnection)
        {
            this.RollBackTransaction(closeConnection);
        }

        #endregion IEasySqlTransactionExecuter

        #region IEasySqlTransactionExecuter

        IDbTransaction IEasySqlTransactionExecuter.BeginTransaction()
        {
            this.BeginTransaction();
            return this.Transaction;
        }

        IDbTransaction IEasySqlTransactionExecuter.BeginTransaction(IsolationLevel level)
        {
            this.BeginTransaction(level);
            return this.Transaction;
        }

        void IEasySqlTransactionExecuter.CommitTransaction()
        {
            this.CommitTransaction();
        }

        void IEasySqlTransactionExecuter.CommitTransaction(bool closeConnection)
        {
            this.CommitTransaction(closeConnection);
        }

        void IEasySqlTransactionExecuter.RollBackTransaction()
        {
            this.RollBackTransaction();
        }

        void IEasySqlTransactionExecuter.RollBackTransaction(bool closeConnection)
        {
            this.RollBackTransaction(closeConnection);
        }

        #endregion IEasySqlTransactionExecuter
    }
}