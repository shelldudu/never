using Never.EasySql.Xml;
using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Never.EasySql
{
    /// <summary>
    /// 基本的读取
    /// </summary>
    public abstract class BaseDaoBuilder : IDaoBuilder, IDisposable
    {
        private class MyDao : BaseDao
        {
            public MyDao(IEasySqlExecuter sqlExecuter, System.Threading.ThreadLocal<ISession> currentSessionThreadLocal) : base(sqlExecuter, currentSessionThreadLocal)
            {
            }
        }

        #region init

        /// <summary>
        /// 所有的流内容
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Stream> GetAllStreams()
        {
            return null;
        }

        /// <summary>
        /// 开启连接
        /// </summary>
        /// <returns></returns>
        protected abstract IEasySqlExecuter CreateSqlExecuter();

        /// <summary>
        /// 返回数据库名字
        /// </summary>
        public abstract string ConnectionString { get; }

        #endregion init

        #region dao

        private bool started = false;

        /// <summary>
        /// 标签提供者
        /// </summary>
        private readonly SqlTagProvider sqlTagProvider = null;

        /// <summary>
        /// 构建者
        /// </summary>
        public virtual Func<IDao> Build
        {
            get
            {
                if (this.started)
                {
                    return this.NewMyDao;
                }

                throw new Exception("the builder not start");
            }
        }

        /// <summary>
        /// 会话管理
        /// </summary>
        private System.Threading.ThreadLocal<ISession> currentSessionThreadLocal = null;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private IDao NewMyDao()
        {
            var session = this.currentSessionThreadLocal.Value;
            if (session != null)
            {
                return session.Dao;
            }
            //System.Threading.Thread.CurrentThread.
            return new MyDao(this.CreateSqlExecuter(), this.currentSessionThreadLocal)
            {
                SqlTagProvider = this.sqlTagProvider,
            };
        }

        #endregion dao

        #region ctor

        /// <summary>
        ///
        /// </summary>
        protected BaseDaoBuilder()
        {
            this.sqlTagProvider = new SqlTagProvider();
        }

        #endregion ctor

        /// <summary>
        /// 开始工作
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public virtual void Startup()
        {
            if (this.started)
            {
                return;
            }

            var streams = this.GetAllStreams();
            if (streams != null)
            {
                streams = streams.Where(o => o != null).ToArray();
                foreach (var i in streams)
                {
                    this.sqlTagProvider.Load(i, null);
                }
            }

            var stringPrefix = string.Empty;
            var sql = this.CreateSqlExecuter();
            {
                var test = sql as IParameterPrefixProvider;
                if (test != null)
                {
                    stringPrefix = test.GetParameterPrefix();
                }
            }

            this.sqlTagProvider.Build(stringPrefix);
            this.OnStarted(streams);
            this.currentSessionThreadLocal = new System.Threading.ThreadLocal<ISession>(false);
            this.started = true;
        }

        /// <summary>
        /// 在启动后
        /// </summary>
        /// <param name="streams"></param>
        protected virtual void OnStarted(IEnumerable<Stream> streams)
        {
            if (streams == null)
            {
                return;
            }

            foreach (var i in streams)
            {
                i.Dispose();
            }
        }

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
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            this.currentSessionThreadLocal.Dispose();
        }
    }
}