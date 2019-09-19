using Never.EasySql.Xml;
using Never.Exceptions;
using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口，该对象每次执行一次都会释放IDao接口，请不要重复使用
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public sealed class EasyDecoratedXmlDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;
        private readonly EasySqlParameter<Parameter> parameter = null;

        #endregion field

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        public EasyDecoratedXmlDao(IDao dao, EasySqlParameter<Parameter> parameter) : base(dao)
        {
            this.dao = dao;
            this.parameter = parameter;
        }

        #endregion ctor

        #region crud

        /// <summary>
        /// 查询某一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectId"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string selectId)
        {
            var tag = this.dao.SqlTagProvider.Get(selectId);
            if (tag == null)
            {
                throw new KeyNotExistedException(selectId, "the sql tag '{0}' not found in the sql files", selectId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForObject<T, Parameter>(tag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForObject<T, Parameter>(tag, this.parameter);
            }
        }

        /// <summary>
        /// 查询可列举的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectId"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string selectId)
        {
            var tag = this.dao.SqlTagProvider.Get(selectId);
            if (tag == null)
            {
                throw new KeyNotExistedException(selectId, "the sql tag '{0}' not found in the sql files", selectId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(tag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(tag, this.parameter);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteId"></param>
        /// <returns></returns>
        public int Delete(string deleteId)
        {
            var tag = this.dao.SqlTagProvider.Get(deleteId);
            if (tag == null)
            {
                throw new KeyNotExistedException(deleteId, "the sql tag '{0}' not found in the sql files", deleteId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.Delete(tag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Delete(tag, this.parameter);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        public int Update(string updateId)
        {
            var tag = this.dao.SqlTagProvider.Get(updateId);
            if (tag == null)
            {
                throw new KeyNotExistedException(updateId, "the sql tag '{0}' not found in the sql files", updateId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.Update(tag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Update(tag, this.parameter);
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="insertId"></param>
        /// <returns></returns>
        public object Insert(string insertId)
        {
            var tag = this.dao.SqlTagProvider.Get(insertId);
            if (tag == null)
            {
                throw new KeyNotExistedException(insertId, "the sql tag '{0}' not found in the sql files", insertId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.Insert(tag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Insert(tag, this.parameter);
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertId"></param>
        /// <returns></returns>
        public T Insert<T>(string insertId)
        {
            var tag = this.dao.SqlTagProvider.Get(insertId);
            if (tag == null)
            {
                throw new KeyNotExistedException(insertId, "the sql tag '{0}' not found in the sql files", insertId);
            }

            if (this.dao.CurrentSession != null)
            {
                return (T)this.dao.Insert(tag, this.parameter);
            }

            using (this.dao)
            {
                return (T)this.dao.Insert(tag, this.parameter);
            }
        }

        /// <summary>
        /// 执行存储更新
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(string callId, CallMode callmode)
        {
            var tag = this.dao.SqlTagProvider.Get(callId);
            if (tag == null)
            {
                throw new KeyNotExistedException(callId, "the sql tag '{0}' not found in the sql files", callId);
            }

            if (this.dao.CurrentSession != null)
            {
                return this.dao.Call(tag, this.parameter, callmode);
            }

            using (this.dao)
            {
                return this.dao.Call(tag, this.parameter, callmode);
            }
        }

        /// <summary>
        /// 获取格式化
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="formatText">是否格式化</param>
        /// <returns></returns>
        public SqlTagFormat GetSqlTagFormat(string sqlId, bool formatText = false)
        {
            var tag = this.dao.SqlTagProvider.Get(sqlId);
            if (tag == null)
            {
                throw new KeyNotExistedException(sqlId, "the sql tag '{0}' not found in the sql files", sqlId);
            }

            return this.dao.GetSqlTagFormat(tag, this.parameter, formatText);
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <returns></returns>
        public ISession BeginTransaction()
        {
            return this.dao.BeginTransaction();
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        public ISession BeginTransaction(IsolationLevel level)
        {
            return this.dao.BeginTransaction(level);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void CommitTransaction()
        {
            this.dao.CommitTransaction();
            this.dao.Dispose();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>

        public void CommitTransaction(bool closeConnection)
        {
            this.dao.CommitTransaction(closeConnection);
            this.dao.Dispose();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackTransaction()
        {
            this.dao.RollBackTransaction();
            this.dao.Dispose();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        public void RollBackTransaction(bool closeConnection)
        {
            this.dao.RollBackTransaction(closeConnection);
            this.dao.Dispose();
        }

        #endregion crud
    }
}