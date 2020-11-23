using Never.EasySql.Xml;
using Never.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 基本的读取数据对象，该对象实例安全，静态实例不安全
    /// </summary>
    public abstract class BaseDao : IDao, IDisposable
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="sqlExecuter"></param>
        /// <param name="currentSessionThreadLocal"></param>
        protected BaseDao(IEasySqlExecuter sqlExecuter, System.Threading.ThreadLocal<ISession> currentSessionThreadLocal)
        {
            this.SqlExecuter = sqlExecuter;
            this.CurrentSessionThreadLocal = currentSessionThreadLocal;
            this.DataSource = new DefaultDataSource()
            {
                ConnectionString = sqlExecuter.ConnectionString,
                ProviderFactory = sqlExecuter.ProviderFactory,
            };
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 执行者
        /// </summary>
        public IEasySqlExecuter SqlExecuter { get; private set; }

        /// <summary>
        /// 当前session的TLS
        /// </summary>
        public System.Threading.ThreadLocal<ISession> CurrentSessionThreadLocal { get; private set; }

        #endregion field

        #region config

        /// <summary>
        /// 数据源管理
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// 标签提供者
        /// </summary>
        public ISqlTagProvider SqlTagProvider { get; set; }

        /// <summary>
        /// 会话管理
        /// </summary>
        public ISession CurrentSession { get; set; }

        #endregion config

        #region trans

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public ISession BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="isolationLevel">事务等级</param>
        /// <returns></returns>
        public virtual ISession BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.CurrentSession != null)
            {
                return this.CurrentSession;
            }

            this.CurrentSession = new DefaultSession()
            {
                Transaction = ((IEasySqlTransactionExecuter)this.SqlExecuter).BeginTransaction(isolationLevel),
                DataSource = DataSource,
                Dao = this,
            };

            this.CurrentSessionThreadLocal.Value = this.CurrentSession;
            return this.CurrentSession;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="closeConnection">是否关闭链接</param>
        public virtual void RollBackTransaction(bool closeConnection)
        {
            if (this.CurrentSession != null)
            {
                ((IEasySqlTransactionExecuter)this.SqlExecuter).RollBackTransaction(closeConnection);
                this.CurrentSessionThreadLocal.Value = null;
            }

            this.CurrentSession.Dispose();
            this.CurrentSession = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBackTransaction()
        {
            this.RollBackTransaction(true);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            this.CommitTransaction(true);
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="closeConnection">是否关闭链接</param>
        public virtual void CommitTransaction(bool closeConnection)
        {
            if (this.CurrentSession != null)
            {
                ((IEasySqlTransactionExecuter)this.SqlExecuter).CommitTransaction(closeConnection);
                this.CurrentSessionThreadLocal.Value = null;
            }

            this.CurrentSession.Dispose();
            this.CurrentSession = null;
        }

        #endregion trans

        #region crud

        /// <summary>
        /// 得到sqltag
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <param name="formatText"></param>
        /// <returns></returns>
        public SqlTagFormat GetSqlTagFormat<T>(SqlTag sqlTag, EasySqlParameter<T> parameter, bool formatText = false)
        {
            return formatText ? sqlTag.FormatForText(parameter) : sqlTag.Format(parameter);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual int Delete<T>(SqlTag sqlTag, EasySqlParameter<T> parameter)
        {
            var format = sqlTag.Format(parameter);
            return this.SqlExecuter.Delete(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual object Insert<T>(SqlTag sqlTag, EasySqlParameter<T> parameter)
        {
            var format = sqlTag.Format(parameter);
            if (format.ReturnType.IsNullOrEmpty())
            {
                return this.SqlExecuter.Insert(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            }

            var @object = this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            switch (format.ReturnType)
            {
                case "byte":
                case "Byte":
                    return Convert.ChangeType(@object, TypeCode.Byte);

                case "sbyte":
                case "SByte":
                    return Convert.ChangeType(@object, TypeCode.SByte);

                case "bool":
                case "Boolean":
                    return Convert.ChangeType(@object, TypeCode.Boolean);

                case "char":
                case "Char":
                    return Convert.ChangeType(@object, TypeCode.Char);

                case "datetime":
                case "DateTime":
                    return Convert.ChangeType(@object, TypeCode.DateTime);

                case "decimal":
                case "Decimal":
                    return Convert.ChangeType(@object, TypeCode.Decimal);

                case "double":
                case "Double":
                    return Convert.ChangeType(@object, TypeCode.Double);

                case "float":
                case "Single":
                    return Convert.ChangeType(@object, TypeCode.Single);

                case "int":
                case "int32":
                    return Convert.ChangeType(@object, TypeCode.Int32);

                case "uint":
                case "UInt32":
                    return Convert.ChangeType(@object, TypeCode.UInt32);

                case "short":
                case "Int16":
                    return Convert.ChangeType(@object, TypeCode.Int16);

                case "ushort":
                case "UInt16":
                    return Convert.ChangeType(@object, TypeCode.UInt16);

                case "long":
                case "Int64":
                    return Convert.ChangeType(@object, TypeCode.Int64);

                case "ulong":
                case "UInt64":
                    return Convert.ChangeType(@object, TypeCode.UInt64);

                case "guid":
                case "Guid":
                    return @object == null ? Guid.Empty : Guid.Parse(@object.ToString());

                case "string":
                case "String":
                    return Convert.ChangeType(@object, TypeCode.String);
            }

            return @object;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual Result QueryForObject<Result, T>(SqlTag sqlTag, EasySqlParameter<T> parameter)
        {
            var format = sqlTag.Format(parameter);
            return this.SqlExecuter.QueryForObject<Result>(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual IEnumerable<Result> QueryForEnumerable<Result, T>(SqlTag sqlTag, EasySqlParameter<T> parameter)
        {
            var format = sqlTag.Format(parameter);
#if DEBUG
            Console.WriteLine(format.ToString());
#endif
            var temp = this.SqlExecuter.QueryForEnumerable<Result>(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            return temp == null ? new Result[0] : temp;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual int Update<T>(SqlTag sqlTag, EasySqlParameter<T> parameter)
        {
            var format = sqlTag.Format(parameter);
            return this.SqlExecuter.Update(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public virtual object Call<T>(SqlTag sqlTag, EasySqlParameter<T> parameter, CallMode callmode)
        {
            var format = sqlTag.Format(parameter);
            if ((callmode & CallMode.ExecuteScalar) == CallMode.ExecuteScalar)
            {
                if ((callmode & CallMode.CommandText) == CallMode.CommandText)
                {
                    return this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.Text, format.Parameters.ToArray());
                }

                if ((callmode & CallMode.CommandStoredProcedure) == CallMode.CommandStoredProcedure)
                {
                    return this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.StoredProcedure, format.Parameters.ToArray());
                }

                if ((callmode & CallMode.CommandTableDirect) == CallMode.CommandTableDirect)
                {
                    return this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.TableDirect, format.Parameters.ToArray());
                }

                return this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            }

            if ((callmode & CallMode.ExecuteNonQuery) == CallMode.ExecuteNonQuery)
            {
                if ((callmode & CallMode.CommandText) == CallMode.CommandText)
                {
                    return this.SqlExecuter.ExecuteNonQuery(format.ToString(), CommandType.Text, format.Parameters.ToArray());
                }

                if ((callmode & CallMode.CommandStoredProcedure) == CallMode.CommandStoredProcedure)
                {
                    return this.SqlExecuter.ExecuteNonQuery(format.ToString(), CommandType.StoredProcedure, format.Parameters.ToArray());
                }

                if ((callmode & CallMode.CommandTableDirect) == CallMode.CommandTableDirect)
                {
                    return this.SqlExecuter.ExecuteNonQuery(format.ToString(), CommandType.TableDirect, format.Parameters.ToArray());
                }

                return this.SqlExecuter.ExecuteNonQuery(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            }

            return this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        #endregion crud

        #region IDisposable

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposed"></param>
        protected void Dispose(bool disposed)
        {
            if (disposed)
            {
                this.SqlExecuter.Dispose();
            }
        }

        #endregion IDisposable
    }
}