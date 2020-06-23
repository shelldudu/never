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
    public abstract class UpdateContext<Parameter, Table> : Context
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
            this.FromTable = this.FormatTable(this.FindTableName<Parameter>(tableInfo));
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
        public virtual UpdateContext<Parameter, Table> From(string table)
        {
            this.FromTable = this.FormatTable(table);
            return this;
        }

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter, Table> As(string table)
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
        /// 入口
        /// </summary>
        public abstract UpdateContext<Parameter, Table> StartSetColumn();

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected abstract string SelectTableNamePointOnSetColunm();

        /// <summary>
        /// 更新字段名
        /// </summary>
        protected abstract UpdateContext<Parameter, Table> SetColumn(string columnName, string originalColumnName, bool textParameter);

        /// <summary>
        /// 更新字段名
        /// </summary>
        public virtual UpdateContext<Parameter, Table> Set<TMember>(Expression<Func<Table, TMember>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            return this.SetColumn(string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), columnName, false);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public virtual UpdateContext<Parameter, Table> SetFunc<TMember>(Expression<Func<Table, TMember>> expression, string value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColumn(string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), columnName, true);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public virtual UpdateContext<Parameter, Table> SetValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColumn(string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), columnName, false);
        }

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter, Table> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter, Table> Where(Expression<Func<Parameter, Table, object>> expression);

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter, Table> Where(AndOrOption andOrOption, string sql);

        /// <summary>
        /// append
        /// </summary>
        public abstract UpdateContext<Parameter, Table> Append(string sql);

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter, Table> JoinOnUpdate(List<JoinInfo> joins);

        /// <summary>
        /// exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter, Table> JoinOnWhereExists(WhereExistsInfo whereExists);

        /// <summary>
        /// in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter, Table> JoinOnWhereIn(WhereInInfo whereIn);
    }
}
