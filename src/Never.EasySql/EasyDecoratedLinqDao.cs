using Never.EasySql.Linq;
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
    public class EasyDecoratedLinqDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;
        private readonly EasySqlParameter<Parameter> parameter = null;
        private readonly Context context = null;
        #endregion field

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        public EasyDecoratedLinqDao(IDao dao, EasySqlParameter<Parameter> parameter) : base(dao)
        {
            this.dao = dao;
            this.parameter = parameter;
            this.context = new Context()
            {
                dao = dao
            };
        }

        #endregion ctor

        #region trans

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <returns></returns>
        public EasyDecoratedLinqDao<Parameter> BeginTransaction()
        {
            this.dao.BeginTransaction();
            return this;
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        public EasyDecoratedLinqDao<Parameter> BeginTransaction(IsolationLevel level)
        {
            this.dao.BeginTransaction(level);
            return this;
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

        #endregion

        #region crud

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public Update<Parameter> Update()
        {
            return new Update<Parameter>();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public Delete<Parameter> Delete()
        {
            return new Delete<Parameter>();
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public Insert<Parameter> Insert()
        {
            return new Insert<Parameter>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T> Select<T>()
        {
            return new Select<Parameter, T>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2> Select<T1, T2>()
        {
            return new Select<Parameter, T1, T2>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <typeparam name="T3">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2, T3> Select<T1, T2, T3>()
        {
            return new Select<Parameter, T1, T2, T3>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <typeparam name="T3">对象</typeparam>
        /// <typeparam name="T4">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2, T3, T4> Select<T1, T2, T3, T4>()
        {
            return new Select<Parameter, T1, T2, T3, T4>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <typeparam name="T3">对象</typeparam>
        /// <typeparam name="T4">对象</typeparam>
        /// <typeparam name="T5">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2, T3, T4, T5> Select<T1, T2, T3, T4, T5>()
        {
            return new Select<Parameter, T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <typeparam name="T3">对象</typeparam>
        /// <typeparam name="T4">对象</typeparam>
        /// <typeparam name="T5">对象</typeparam>
        /// <typeparam name="T6">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2, T3, T4, T5, T6> Select<T1, T2, T3, T4, T5, T6>()
        {
            return new Select<Parameter, T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象</typeparam>
        /// <typeparam name="T3">对象</typeparam>
        /// <typeparam name="T4">对象</typeparam>
        /// <typeparam name="T5">对象</typeparam>
        /// <typeparam name="T6">对象</typeparam>
        /// <typeparam name="T7">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, T1, T2, T3, T4, T5, T6, T7> Select<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new Select<Parameter, T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(string sql, Parameter @parameter, CallMode callmode)
        {
            var sqlTag = EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao);
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(Action<Parameter, StringBuilder> sql, Parameter @parameter, CallMode callmode = CallMode.ExecuteScalar | CallMode.CommandText)
        {
            var sb = new StringBuilder();
            sql(parameter, sb);
            return this.Call(sb.ToString(), @parameter, callmode);
        }

        #endregion crud
    }
}
