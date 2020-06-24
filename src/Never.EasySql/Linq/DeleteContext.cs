using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 删除上下文
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public abstract class DeleteContext<Table,Parameter> : Context
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
        protected DeleteContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract int GetResult();

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract DeleteContext<Table,Parameter> From(string table);

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract DeleteContext<Table,Parameter> AsTable(string table);

        /// <summary>
        /// 入口
        /// </summary>
        public abstract DeleteContext<Table,Parameter> Entrance();

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Delete(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行更新（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.Delete(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return -1;
            }
        }

        /// <summary>
        /// where
        /// </summary>
        public abstract DeleteContext<Table,Parameter> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract DeleteContext<Table,Parameter> Where(Expression<Func<Parameter, object>> expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> Exists<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> NotExists<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> In<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> NotIn<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> Exists(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> NotExists(AndOrOption option, string expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> In(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract DeleteContext<Table,Parameter> NotIn(AndOrOption option, string expression);
    }
}
