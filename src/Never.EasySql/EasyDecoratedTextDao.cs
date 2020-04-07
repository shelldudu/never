using Never.EasySql.Text;
using Never.EasySql.Xml;
using Never.SqlClient;
using Never.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Never.EasySql
{
    /// <summary>
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口，该对象每次执行一次都会释放IDao接口，请不要重复使用
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public sealed class EasyDecoratedTextDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;
        private readonly EasySqlParameter<Parameter> parameter = null;
        private string cacheId = null;
        #endregion field

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        public EasyDecoratedTextDao(IDao dao,
            EasySqlParameter<Parameter> parameter) : base(dao)
        {
            this.dao = dao;
            this.parameter = parameter;
        }

        #endregion ctor

        #region crud

        /// <summary>
        /// 将sql语句分析好后缓存起来
        /// </summary>
        /// <param name="cacheId"></param>
        /// <returns></returns>
        public EasyDecoratedTextDao<Parameter> Cached(string cacheId)
        {
            this.cacheId = cacheId;
            return this;
        }

        /// <summary>
        /// 查询某一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql,this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
            }
        }

        /// <summary>
        /// 查询可列举的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
            }
        }

        /// <summary>
        /// 查询可列举的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.QueryForEnumerable<T>(sb.ToString());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Delete<Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Delete<Parameter>(sqlTag, this.parameter);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Delete(sb.ToString());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Update<Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Update<Parameter>(sqlTag, this.parameter);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Update(sb.ToString());
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object Insert(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Insert<Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return this.dao.Insert<Parameter>(sqlTag, this.parameter);
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T Insert<T>(string sql)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return (T)this.dao.Insert<Parameter>(sqlTag, this.parameter);
            }

            using (this.dao)
            {
                return (T)this.dao.Insert<Parameter>(sqlTag, this.parameter);
            }

        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object Insert(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Insert(sb.ToString());
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(string sql, CallMode callmode)
        {
            var sqlTag = TextSqlTagBuilder.Build(sql, this.cacheId, this.dao);
            if (this.dao.CurrentSession != null)
            {
                return this.dao.Call<Parameter>(sqlTag, this.parameter, callmode);
            }

            using (this.dao)
            {
                return this.dao.Call<Parameter>(sqlTag, this.parameter, callmode);
            }
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