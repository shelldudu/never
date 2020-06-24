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
    public abstract class SelectContext<Table,Parameter> : Context
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
        /// last labes;
        /// </summary>
        protected List<ILabel> lastLabels;

        /// <summary>
        /// 临时参数
        /// </summary>
        protected readonly Dictionary<string, object> templateParameter;

        /// <summary>
        /// order by
        /// </summary>
        protected List<OrderByInfo> orderBies;

        /// <summary>
        /// 从哪个表
        /// </summary>
        public string FromTable
        {
            get; protected set;
        }

        /// <summary>
        /// select join
        /// </summary>
        protected List<JoinInfo> selectJoin;

        /// <summary>
        /// where exists
        /// </summary>
        protected WhereExistsInfo whereExists;

        /// <summary>
        /// where in
        /// </summary>
        protected WhereInInfo whereIn;

        /// <summary>
        /// 别名
        /// </summary>
        public string AsTable
        {
            get; protected set;
        }

        /// <summary>
        /// 是否单条记录
        /// </summary>
        protected bool isSingle;

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
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual SelectContext<Table,Parameter> From(string table)
        {
            this.FromTable = this.FormatTable(table);
            return this;
        }

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual SelectContext<Table,Parameter> As(string table)
        {
            this.AsTable = table;
            return this;
        }

        /// <summary>
        /// 检查名称是否合格
        /// </summary>
        /// <param name="tableName"></param>
        public virtual void CheckTableNameIsExists(string tableName)
        {
            if (this.FromTable.IsEquals(tableName))
                throw new Exception(string.Format("the table name {0} is equal alias Name {1}", this.FromTable, tableName));

            if (this.AsTable.IsEquals(tableName))
                throw new Exception(string.Format("the table alias name {0} is equal alias Name {1}", this.AsTable, tableName));
        }

        /// <summary>
        /// 设为单条
        /// </summary>
        /// <returns></returns>
        public SelectContext<Table,Parameter> SetSingle()
        {
            this.isSingle = true;
            return this;
        }

        /// <summary>
        /// 设为单条
        /// </summary>
        /// <returns></returns>
        public SelectContext<Table,Parameter> SetPage()
        {
            this.isSingle = false;
            return this;
        }

        /// <summary>
        /// 入口
        /// </summary>
        public abstract SelectContext<Table,Parameter> StartSelectColumn();

        /// <summary>
        /// 在select的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected abstract string SelectTableNamePointOnSelectColunm();

        /// <summary>
        /// 更新字段名
        /// </summary>
        protected abstract SelectContext<Table,Parameter> SelectColumn(string columnName, string originalColunmName, string @as);

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public virtual SelectContext<Table,Parameter> SelectAll()
        {
            return this.SelectColumn(string.Concat(this.SelectTableNamePointOnSelectColunm(), "*"), "*", string.Empty);
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public virtual SelectContext<Table,Parameter> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            return this.SelectColumn(string.Concat(this.SelectTableNamePointOnSelectColunm(), this.FormatColumn(columnName)), columnName, string.Empty);
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public virtual SelectContext<Table,Parameter> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            return this.SelectColumn(string.Concat(this.SelectTableNamePointOnSelectColunm(), this.FormatColumn(columnName)), columnName, @as);
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public virtual SelectContext<Table,Parameter> Select(string func, string @as)
        {
            return this.SelectColumn(func, func, @as);
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
        public abstract IEnumerable<Table> GetResults(int startIndex, int endIndex);

        /// <summary>
        /// order by
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectContext<Table,Parameter> OrderBy(Expression<Func<Table, object>> expression)
        {
            string @as = this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable;
            if (this.orderBies == null)
                this.orderBies = new List<OrderByInfo>(1);

            this.orderBies.Add(new OrderByInfo()
            {
                OrderBy = expression,
                Placeholder = @as,
                Type = typeof(Table),
                Flag = "asc"
            });

            return this;
        }

        /// <summary>
        /// order by
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectContext<Table,Parameter> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            string @as = this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable;
            if (this.orderBies == null)
                this.orderBies = new List<OrderByInfo>(1);

            this.orderBies.Add(new OrderByInfo()
            {
                OrderBy = expression,
                Placeholder = @as,
                Type = typeof(Table),
                Flag = "desc"
            });

            return this;
        }

        /// <summary>
        /// order by
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal SelectContext<Table,Parameter> OrderBy<Table1>(Expression<Func<Table1, object>> expression, int index)
        {
            string @as = this.selectJoin[index].AsName;
            if (this.orderBies == null)
                this.orderBies = new List<OrderByInfo>(1);

            this.orderBies.Add(new OrderByInfo()
            {
                OrderBy = expression,
                Placeholder = @as,
                Type = typeof(Table1),
                Flag = "asc"
            });

            return this;
        }

        /// <summary>
        /// order by
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal SelectContext<Table,Parameter> OrderByDescending<Table1>(Expression<Func<Table1, object>> expression, int index)
        {
            string @as = this.selectJoin[index].AsName;
            if (this.orderBies == null)
                this.orderBies = new List<OrderByInfo>(1);

            this.orderBies.Add(new OrderByInfo()
            {
                OrderBy = expression,
                Placeholder = @as,
                Type = typeof(Table1),
                Flag = "desc"
            });

            return this;
        }

        /// <summary>
        /// where
        /// </summary>
        public abstract SelectContext<Table,Parameter> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract SelectContext<Table,Parameter> Where(Expression<Func<Table,Parameter, object>> expression);

        /// <summary>
        /// append
        /// </summary>
        public abstract SelectContext<Table,Parameter> Append(string sql);

        /// <summary>
        /// last
        /// </summary>
        public abstract SelectContext<Table,Parameter> Last(string sql);

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public SelectContext<Table,Parameter> JoinSelect(List<JoinInfo> joins)
        {
            this.selectJoin = joins;
            return this.OnJoinSelect();
        }

        /// <summary>
        /// join
        /// </summary>
        /// <returns></returns>
        protected virtual SelectContext<Table,Parameter> OnJoinSelect()
        {
            return this;
        }

        /// <summary>
        /// where里面的exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public abstract SelectContext<Table,Parameter> AppenInWhereExists(WhereExistsInfo whereExists);

        /// <summary>
        /// where里面的in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public abstract SelectContext<Table,Parameter> AppenInWhereIn(WhereInInfo whereIn);

        /// <summary>
        /// exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public SelectContext<Table,Parameter> WhereExists(WhereExistsInfo whereExists)
        {
            this.whereExists = whereExists;
            return this.OnWhereExists();
        }

        /// <summary>
        /// exists
        /// </summary>
        /// <returns></returns>
        protected virtual SelectContext<Table,Parameter> OnWhereExists()
        {
            return this;
        }

        /// <summary>
        /// in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public SelectContext<Table,Parameter> WhereIn(WhereInInfo whereIn)
        {
            this.whereIn = whereIn;
            return this.OnWhereIn();
        }


        /// <summary>
        /// in
        /// </summary>
        /// <returns></returns>
        protected virtual SelectContext<Table,Parameter> OnWhereIn()
        {
            return this;
        }
    }
}
