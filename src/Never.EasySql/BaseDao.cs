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
        /// 获取Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual SqlTag GetSqlTag(string id)
        {
            return this.SqlTagProvider.Get(id);
        }

        /// <summary>
        /// 得到sqltag
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="parameter"></param>
        /// <param name="formatText"></param>
        /// <returns></returns>
        public SqlTagFormat GetSqlTagFormat<T>(string sqlId, EasySqlParameter<T> parameter, bool formatText = false)
        {
            var tag = this.GetSqlTag(sqlId);
            if (tag == null)
            {
                throw new KeyNotExistedException(sqlId, "the sql tag '{0}' not found in the sql files", sqlId);
            }

            return formatText ? tag.FormatForText(parameter) : tag.Format(parameter);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int Delete<T>(string deleteId, EasySqlParameter<T> parameter)
        {
            var tag = this.GetSqlTag(deleteId);
            if (tag == null)
            {
                throw new KeyNotExistedException(deleteId, "the sql tag '{0}' not found in the sql files", deleteId);
            }

            return this.Delete<T>(tag, parameter);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual int Delete<T>(SqlTag tag, EasySqlParameter<T> parameter)
        {
            var format = tag.Format(parameter);
            return this.SqlExecuter.Delete(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object Insert<T>(string insertId, EasySqlParameter<T> parameter)
        {
            var tag = this.GetSqlTag(insertId);
            if (tag == null)
            {
                throw new KeyNotExistedException(insertId, "the sql tag '{0}' not found in the sql files", insertId);
            }

            return this.Insert<T>(tag, parameter);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual object Insert<T>(SqlTag tag, EasySqlParameter<T> parameter)
        {
            var format = tag.Format(parameter);
            if (format.ReturnType.IsNullOrEmpty())
            {
                return this.SqlExecuter.Insert(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            }

            var @object = this.SqlExecuter.ExecuteScalar(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            switch (format.ReturnType)
            {
                case "byte":
                    return Convert.ChangeType(@object, TypeCode.Byte);

                case "sbyte":
                    return Convert.ChangeType(@object, TypeCode.SByte);

                case "bool":
                    return Convert.ChangeType(@object, TypeCode.Boolean);

                case "char":
                    return Convert.ChangeType(@object, TypeCode.Char);

                case "datetime":
                    return Convert.ChangeType(@object, TypeCode.DateTime);

                case "decimal":
                    return Convert.ChangeType(@object, TypeCode.Decimal);

                case "double":
                    return Convert.ChangeType(@object, TypeCode.Double);

                case "float":
                    return Convert.ChangeType(@object, TypeCode.Single);

                case "int":
                    return Convert.ChangeType(@object, TypeCode.Int32);

                case "uint":
                    return Convert.ChangeType(@object, TypeCode.UInt32);

                case "short":
                    return Convert.ChangeType(@object, TypeCode.Int16);

                case "ushort":
                    return Convert.ChangeType(@object, TypeCode.UInt16);

                case "long":
                    return Convert.ChangeType(@object, TypeCode.Int64);

                case "ulong":
                    return Convert.ChangeType(@object, TypeCode.UInt64);

                case "guid":
                    return @object == null ? Guid.Empty : Guid.Parse(@object.ToString());

                case "string":
                    return Convert.ChangeType(@object, TypeCode.String);
            }

            return @object;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Result QueryForObject<Result, T>(string selectId, EasySqlParameter<T> parameter)
        {
            var tag = this.GetSqlTag(selectId);
            if (tag == null)
            {
                throw new KeyNotExistedException(selectId, "the sql tag '{0}' not found in the sql files", selectId);
            }

            return this.QueryForObject<Result, T>(tag, parameter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual Result QueryForObject<Result, T>(SqlTag tag, EasySqlParameter<T> parameter)
        {
            var format = tag.Format(parameter);
            return this.SqlExecuter.QueryForObject<Result>(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<Result> QueryForEnumerable<Result, T>(string selectId, EasySqlParameter<T> parameter)
        {
            var tag = this.GetSqlTag(selectId);
            if (tag == null)
            {
                throw new KeyNotExistedException(selectId, "the sql tag '{0}' not found in the sql files", selectId);
            }

            return this.QueryForEnumerable<Result, T>(tag, parameter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual IEnumerable<Result> QueryForEnumerable<Result, T>(SqlTag tag, EasySqlParameter<T> parameter)
        {
            var format = tag.Format(parameter);
            var temp = this.SqlExecuter.QueryForEnumerable<Result>(format.ToString(), CommandType.Text, format.Parameters.ToArray());
            return temp == null ? new Result[0] : temp;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int Update<T>(string updateId, EasySqlParameter<T> parameter)
        {
            var tag = this.GetSqlTag(updateId);
            if (tag == null)
            {
                throw new KeyNotExistedException(updateId, "the sql tag '{0}' not found in the sql files", updateId);
            }

            return this.Update<T>(tag, parameter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual int Update<T>(SqlTag tag, EasySqlParameter<T> parameter)
        {
            var format = tag.Format(parameter);
            return this.SqlExecuter.Update(format.ToString(), CommandType.Text, format.Parameters.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callId"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call<T>(string callId, EasySqlParameter<T> parameter, CallMode callmode)
        {
            var tag = this.GetSqlTag(callId);
            if (tag == null)
            {
                throw new KeyNotExistedException(callId, "the sql tag '{0}' not found in the sql files", callId);
            }

            return this.Call<T>(tag, parameter, callmode);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public virtual object Call<T>(SqlTag tag, EasySqlParameter<T> parameter, CallMode callmode)
        {
            var format = tag.Format(parameter);
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