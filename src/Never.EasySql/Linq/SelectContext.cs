using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 查询上下文
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public abstract class SelectContext<Parameter, Table> : Context
    {
        /// <summary>
        /// dao
        /// </summary>
        protected readonly IDao dao;

        /// <summary>
        /// tableInfo
        /// </summary>
        protected readonly TableInfo tableInfo;

        /// <summary>
        /// sqlparameter
        /// </summary>
        protected readonly EasySqlParameter<Parameter> sqlParameter;

        /// <summary>
        /// labels
        /// </summary>
        protected readonly List<ILabel> labels;

        /// <summary>
        /// 临时参数
        /// </summary>
        protected readonly Dictionary<string, object> templateParameter;

        /// <summary>
        /// 是否单条记录
        /// </summary>
        protected bool isSingle;

        /// <summary>
        /// 分页
        /// </summary>
        protected PagedSearch paged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected SelectContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// from table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract SelectContext<Parameter, Table> From(string table);

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract SelectContext<Parameter, Table> AsTable(string table);

        /// <summary>
        /// 入口
        /// </summary>
        public abstract SelectContext<Parameter, Table> Entrance();

        /// <summary>
        /// 设为单条
        /// </summary>
        /// <returns></returns>
        public SelectContext<Parameter, Table> SetSingle()
        {
            this.isSingle = true;
            this.paged = null;
            return this;
        }

        /// <summary>
        /// 设为单条
        /// </summary>
        /// <returns></returns>
        public SelectContext<Parameter, Table> SetPage(PagedSearch paged)
        {
            this.isSingle = false;
            this.paged = paged;
            return this;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Table Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.QueryForObject<Table, Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行查询（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Table Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.QueryForObject<Table, Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(Table);
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected IEnumerable<Table> Execute2(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.QueryForEnumerable<Table, Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行查询（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected IEnumerable<Table> Execute2(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.QueryForEnumerable<Table, Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(IEnumerable<Table>);
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public abstract Table GetResult();

        /// <summary>
        /// 获取数组结果
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Table> GetResults();

        /// <summary>
        /// where
        /// </summary>
        public abstract SelectContext<Parameter, Table> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> Exists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> NotExists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> In<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> NotIn<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> Exists(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> NotExists(AndOrOption option, string expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> In(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract SelectContext<Parameter, Table> NotIn(AndOrOption option, string expression);
    }
}
