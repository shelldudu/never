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
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口
    /// </summary>
    public class EasyDecoratedDao : IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyDecoratedDao"/> class.
        /// </summary>
        /// <param name="dao">The DAO.</param>
        protected EasyDecoratedDao(IDao dao)
        {
            this.dao = dao;
        }

        #endregion ctor

        #region IDao

        /// <summary>
        /// 数据源一些配置
        /// </summary>
        public IDataSource DataSource => this.dao.DataSource;

        /// <summary>
        /// 开启事务后一些会话
        /// </summary>
        public ISession CurrentSession => this.dao.CurrentSession;

        /// <summary>
        /// Sql执行者
        /// </summary>
        public IEasySqlExecuter SqlExecuter => this.dao.SqlExecuter;

        /// <summary>
        /// sqlTag提供者
        /// </summary>
        public ISqlTagProvider SqlTagProvider => this.dao.SqlTagProvider;

        object IDao.Call<T>(string deleteId, EasySqlParameter<T> parameter, CallMode callmode)
        {
            return this.dao.Call<T>(deleteId, parameter, callmode);
        }

        int IDao.Delete<T>(string deleteId, EasySqlParameter<T> parameter)
        {
            return this.dao.Delete<T>(deleteId, parameter);
        }

        object IDao.Insert<T>(string insertId, EasySqlParameter<T> parameter)
        {
            return this.dao.Insert<T>(insertId, parameter);
        }

        Result IDao.QueryForObject<Result, T>(string selectId, EasySqlParameter<T> parameter)
        {
            return this.dao.QueryForObject<Result, T>(selectId, parameter);
        }

        IEnumerable<Result> IDao.QueryForEnumerable<Result, T>(string selectId, EasySqlParameter<T> parameter)
        {
            return this.dao.QueryForEnumerable<Result, T>(selectId, parameter);
        }

        int IDao.Update<T>(string updateId, EasySqlParameter<T> parameter)
        {
            return this.dao.Update<T>(updateId, parameter);
        }

        ISession IDao.BeginTransaction()
        {
            return this.dao.BeginTransaction();
        }

        ISession IDao.BeginTransaction(IsolationLevel level)
        {
            return this.dao.BeginTransaction(level);
        }

        void IDao.CommitTransaction()
        {
            this.dao.CommitTransaction();
        }

        void IDao.CommitTransaction(bool closeConnection)
        {
            this.dao.CommitTransaction(closeConnection);
        }

        void IDao.RollBackTransaction()
        {
            this.dao.RollBackTransaction();
        }

        void IDao.RollBackTransaction(bool closeConnection)
        {
            this.dao.RollBackTransaction(closeConnection);
        }

        SqlTagFormat IDao.GetSqlTagFormat<T>(string sqlId, EasySqlParameter<T> parameter, bool formatText)
        {
            return this.dao.GetSqlTagFormat<T>(sqlId, parameter, formatText);
        }

        #endregion IDao

        #region dispose

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (this.dao != null)
            {
                if (this.dao.SqlExecuter != null)
                {
                    this.dao.SqlExecuter.Dispose();
                }

                this.dao.Dispose();
            }
        }

        #endregion dispose
    }
}