using Never.EasySql.Client;
using Never.EasySql.Linq;
using Never.EasySql.Text;
using Never.EasySql.Xml;
using Never.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口，该对象每次执行一次都会释放IDao接口，请不要重复使用
    /// </summary>
    public sealed class EasyDecoratedLinqDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
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
        public EasyDecoratedLinqDao(IDao dao, EasySqlParameter<Parameter> parameter) : base(dao)
        {
            this.dao = dao;
            this.parameter = parameter;
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
        /// 将sql语句分析好后缓存起来
        /// </summary>
        /// <param name="cacheId"></param>
        /// <returns></returns>
        public EasyDecoratedLinqDao<Parameter> Cached(string cacheId)
        {
            this.cacheId = cacheId;
            return this;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public Update<Parameter> Update()
        {
            LinqSqlTag tag = null;
            if (this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);


            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = tag == null ?
                    new Linq.MySql.UpdateContext<Parameter>(this.cacheId, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter) :
                    (UpdateContext<Parameter>)new UpdatedContext<Parameter>(tag, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Update<Parameter>()
            {
                Context = tag == null ?
                new UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter) :
                (UpdateContext<Parameter>)new UpdatedContext<Parameter>(tag, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter)
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public Delete<Parameter> Delete()
        {
            LinqSqlTag tag = null;
            if (this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = tag == null ?
                    new Linq.MySql.DeleteContext<Parameter>(this.cacheId, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter) :
                    (DeleteContext<Parameter>)new DeletedContext<Parameter>(tag, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Delete<Parameter>()
            {
                Context = tag == null ?
                new DeletingContext<Parameter>(this.cacheId, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter) :
                (DeleteContext<Parameter>)new DeletedContext<Parameter>(tag, this, Linq.Context.GetTableInfo<Parameter>(), this.parameter)
            };
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public Insert<Parameter> Insert()
        {
            LinqSqlTag tag = null;
            if (this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);


            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = tag == null ?
                    new Linq.MySql.InsertContext<Parameter>(this.cacheId, this, Context.GetTableInfo<Parameter>(), this.parameter) :
                    (InsertContext<Parameter>)new InsertedContext<Parameter>(tag, this, Context.GetTableInfo<Parameter>(), this.parameter)
                };

            }

            return new Insert<Parameter>()
            {
                Context = tag == null ?
                new InsertingContext<Parameter>(this.cacheId, this, Context.GetTableInfo<Parameter>(), this.parameter) :
                (InsertContext<Parameter>)new InsertedContext<Parameter>(tag, this, Context.GetTableInfo<Parameter>(), this.parameter)
            };
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="Table">对象</typeparam>
        /// <returns></returns>
        public Select<Parameter, Table> Select<Table>()
        {
            LinqSqlTag tag = null;
            if (this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);


            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context =
                    tag == null ? new Linq.MySql.SelectContext<Parameter, Table>(this.cacheId, this, Context.GetTableInfo<Table>(), this.parameter) :
                    (SelectContext<Parameter, Table>)new SelectedContext<Parameter, Table>(tag, this, Context.GetTableInfo<Table>(), this.parameter)
                };
            }

            return new Select<Parameter, Table>()
            {
                Context =
                tag == null ? new SelectingContext<Parameter, Table>(this.cacheId, this, Context.GetTableInfo<Table>(), this.parameter) :
                (SelectContext<Parameter, Table>)new SelectedContext<Parameter, Table>(tag, this, Context.GetTableInfo<Table>(), this.parameter)
            };
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

        #endregion crud
    }
}
