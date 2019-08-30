using Never.EasySql.Xml;
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
    public class EasyDecoratedLinqDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
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
        public EasyDecoratedLinqDao(IDao dao,
            EasySqlParameter<Parameter> parameter) : base(dao)
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
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql)
        {
            return this.QueryForObject<T>(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private T QueryForObject<T>(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return sqlDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
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
            return this.QueryForEnumerable<T>(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private IEnumerable<T> QueryForEnumerable<T>(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
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
            return this.Delete(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private int Delete(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
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
            return this.Update(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private int Update(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
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
            return this.Insert(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T Insert<T>(string sql)
        {
            return (T)this.Insert(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private object Insert(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
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
            return this.Call(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao), callmode);
        }

        private object Call(string sql, SqlTag sqlTag, CallMode callmode)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(Action<Parameter, StringBuilder> sql, CallMode callmode = CallMode.ExecuteScalar | CallMode.CommandText)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Call(sb.ToString(), callmode);
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
