using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 插入上下文
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public abstract class InsertContext<Parameter, Table> : Context
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
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected InsertContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> Into(string table);

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> Entrance(char flag);

        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected T Execute<T>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return (T)dao.Insert<Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行插入（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected T Execute<T>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = (T)dao.Insert<Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(T);
            }
        }

        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected void Execute2(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            dao.Insert<Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行插入（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected void Execute2(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                dao.Insert<Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
            }
            catch
            {
                dao.RollBackTransaction();
                return;
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract Result GetResult<Result>();

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract void GetResult();

        /// <summary>
        /// 最后自增字符串
        /// </summary>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> InsertLastInsertId();

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> expression);

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> expression, string function);

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value);

    }

}
