using Never.EasySql.Xml;
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
            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForObject<T, Parameter>(selectId, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForObject<T, Parameter>(selectId, this.parameter);
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
            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(selectId, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(selectId, this.parameter);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteId"></param>
        /// <returns></returns>
        public int Delete(string deleteId)
        {
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Delete(deleteId, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Delete(deleteId, this.parameter);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        public int Update(string updateId)
        {
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Update(updateId, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Update(updateId, this.parameter);
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="insertSqlId"></param>
        /// <returns></returns>
        public object Insert(string insertSqlId)
        {
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Insert(insertSqlId, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Insert(insertSqlId, this.parameter);
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertSqlId"></param>
        /// <returns></returns>
        public T Insert<T>(string insertSqlId)
        {
            if (this.dao.CurrentSession != null)
            {
                return (T)this.dao.Insert(insertSqlId, this.parameter);
            }

            using (this.dao)
            {
                return (T)this.dao.Insert(insertSqlId, this.parameter);
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
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Call(callId, this.parameter, callmode);
            }

            using (this.dao)
            {
                return this.dao.Call(callId, this.parameter, callmode);
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
            return this.dao.GetSqlTagFormat(sqlId, this.parameter, formatText);
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