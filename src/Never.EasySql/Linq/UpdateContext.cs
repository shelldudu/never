using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新操作上下文
    /// </summary>
    public abstract class UpdateContext<Parameter> : Context
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
        /// 从哪个表
        /// </summary>
        public string FromTable
        {
            get; protected set;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public string AsTable
        {
            get; protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected UpdateContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao;
            this.tableInfo = tableInfo;
            this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.FromTable = this.Format(this.FindTableName<Parameter>(tableInfo));
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Update(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行更新（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.Update(sqlTag, sqlParameter);
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
        /// 获取结果
        /// </summary>
        public abstract int GetResult();

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> From(string table)
        {
            this.FromTable = this.Format(table);
            return this;
        }

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> As(string table)
        {
            this.AsTable = table;
            return this;
        }
        
        /// <summary>
        /// 检查名称是否合格
        /// </summary>
        /// <param name="tableName"></param>
        public void CheckTableNameIsExists(string tableName) 
        {
            if (this.FromTable.IsEquals(tableName))
                throw new Exception(string.Format("the table name {0} is equal alias Name {1}", this.FromTable, tableName));

            if (this.AsTable.IsEquals(tableName))
                throw new Exception(string.Format("the table alias name {0} is equal alias Name {1}", this.AsTable, tableName));
        }

        /// <summary>
        /// 入口
        /// </summary>
        public abstract UpdateContext<Parameter> StartSetColumn();

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected abstract string SelectTableNamePointOnSetolunm();

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value);

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression);

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter> Where(AndOrOption andOrOption, string sql);

        /// <summary>
        /// end
        /// </summary>
        public abstract UpdateContext<Parameter> End(string sql);

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter> JoinOnUpdate(List<JoinInfo> joins);

        /// <summary>
        /// exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter> JoinOnWhereExists(WhereExists whereExists);

        /// <summary>
        /// in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter> JoinOnWhereIn(WhereIn whereIn);
    }
}
