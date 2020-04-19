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

            if (tag != null)
            {
                return new Update<Parameter>()
                {
                    Context = new UpdatedContext<Parameter>(tag, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.MySql.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.Odbc.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.OleDb.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.Oracle.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.Oracle.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.Postgre.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.Sqlite.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Update<Parameter>()
                {
                    Context = new Linq.SqlServer.UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Update<Parameter>()
            {
                Context = new UpdatingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
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

            if (tag != null)
            {
                return new Delete<Parameter>()
                {
                    Context = new DeletedContext<Parameter>(tag, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.MySql.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.Odbc.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.OleDb.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.Oracle.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.Oracle.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.Postgre.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.Sqlite.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Delete<Parameter>()
                {
                    Context = new Linq.SqlServer.DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Delete<Parameter>()
            {
                Context = new DeletingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
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

            if (tag != null)
            {
                return new Insert<Parameter>()
                {
                    Context = new InsertedContext<Parameter>(tag, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.MySql.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.Odbc.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.OleDb.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.Oracle.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.Oracle.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.Postgre.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.Sqlite.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Insert<Parameter>()
                {
                    Context = new Linq.SqlServer.InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Insert<Parameter>()
            {
                Context = new InsertingContext<Parameter>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
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

            if (tag != null)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new SelectedContext<Parameter, Table>(tag, this, Context.FindTableInfo<Table>(), this.parameter)
                };
            }

            if (this.SqlExecuter is MySqlExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.MySql.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OdbcServerExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.Odbc.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OleDbServerExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.OleDb.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.Oracle.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }


            if (this.SqlExecuter is OracleServerExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.Oracle.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is PostgreSqlExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.Postgre.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqliteExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.Sqlite.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            if (this.SqlExecuter is SqlServerExecuter)
            {
                return new Select<Parameter, Table>()
                {
                    Context = new Linq.SqlServer.SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
                };
            }

            return new Select<Parameter, Table>()
            {
                Context = new SelectingContext<Parameter, Table>(this.cacheId, this, Linq.Context.FindTableInfo<Parameter>(), this.parameter)
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
