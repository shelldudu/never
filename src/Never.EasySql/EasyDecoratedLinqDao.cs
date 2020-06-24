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
        private LinqSqlTag tag;
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
        /// 是否已经构建了
        /// </summary>
        public bool Builded
        {
            get
            {
                if (this.cacheId.IsNullOrEmpty())
                    return true;

                return LinqSqlTagProvider.Get(this.cacheId, out tag);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="Table">对象</typeparam>
        /// <returns></returns>
        public Update<Table, Parameter> Update<Table>()
        {
            if (tag == null && this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);

            if (tag != null)
            {
                if (tag.Owner.Equals("update") == false)
                    throw new Exception(string.Format("the cachedid {0} owner is {1}", this.cacheId, tag.Owner));

                return new Update<Table, Parameter>()
                {
                    Context = new UpdatedContext<Table, Parameter>(tag, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.MySql.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.Odbc.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.OleDb.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.Oracle.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.Oracle.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.Postgre.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.Sqlite.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Update<Table, Parameter>()
                {
                    Context = new Linq.SqlServer.UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            return new Update<Table, Parameter>()
            {
                Context = new UpdatingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
            };
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public Update<Parameter, Parameter> Update()
        {
            return this.Update<Parameter>();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="Table">对象</typeparam>
        /// <returns></returns>
        public Delete<Table, Parameter> Delete<Table>()
        {
            if (tag == null && this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);

            if (tag != null)
            {
                if (tag.Owner.Equals("delete") == false)
                    throw new Exception(string.Format("the cachedid {0} owner is {1}", this.cacheId, tag.Owner));

                return new Delete<Table, Parameter>()
                {
                    Context = new DeletedContext<Table, Parameter>(tag, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.MySql.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.Odbc.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.OleDb.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.Oracle.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.Oracle.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.Postgre.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.Sqlite.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Delete<Table, Parameter>()
                {
                    Context = new Linq.SqlServer.DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            return new Delete<Table, Parameter>()
            {
                Context = new DeletingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public Delete<Parameter, Parameter> Delete()
        {
            return this.Delete<Parameter>();
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="Table">对象</typeparam>
        /// <returns></returns>
        public Insert<Table, Parameter> Insert<Table>()
        {
            if (tag == null && this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);

            if (tag != null)
            {
                if (tag.Owner.Equals("insert") == false)
                    throw new Exception(string.Format("the cachedid {0} owner is {1}", this.cacheId, tag.Owner));

                return new Insert<Table, Parameter>()
                {
                    Context = new InsertedContext<Table, Parameter>(tag, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.MySql.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.Odbc.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.OleDb.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.Oracle.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.Oracle.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.Postgre.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.Sqlite.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Insert<Table, Parameter>()
                {
                    Context = new Linq.SqlServer.InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            return new Insert<Table, Parameter>()
            {
                Context = new InsertingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
            };
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public Insert<Parameter, Parameter> Insert()
        {
            return this.Insert<Parameter>();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="Table">对象</typeparam>
        /// <returns></returns>
        public Select<Table, Parameter> Select<Table>()
        {
            if (tag == null && this.cacheId.IsNotNullOrEmpty())
                LinqSqlTagProvider.Get(this.cacheId, out tag);

            if (tag != null)
            {
                if (tag.Owner.Equals("select") == false)
                    throw new Exception(string.Format("the cachedid {0} owner is {1}", this.cacheId, tag.Owner));

                return new Select<Table, Parameter>()
                {
                    Context = new SelectedContext<Table, Parameter>(tag, this, Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.MySql.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.Odbc.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.OleDb.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.Oracle.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.Oracle.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.Postgre.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.Sqlite.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Select<Table, Parameter>()
                {
                    Context = new Linq.SqlServer.SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            return new Select<Table, Parameter>()
            {
                Context = new SelectingContext<Table, Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Table>(), this.parameter)
            };
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public Select<Parameter, Parameter> Select()
        {
            return this.Select<Parameter>();
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
