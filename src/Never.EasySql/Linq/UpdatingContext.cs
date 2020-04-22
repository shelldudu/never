using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新语法
    /// </summary>
    public class UpdatingContext<Parameter> : UpdateContext<Parameter>
    {
        #region prop
        /// <summary>
        /// 缓存Id
        /// </summary>
        protected readonly string cacheId;
        /// <summary>
        /// 总字符串长度
        /// </summary>
        protected int textLength;
        /// <summary>
        /// set出现的的次数
        /// </summary>
        protected int setTimes;

        /// <summary>
        /// tableName.
        /// </summary>
        protected string tableNamePoint;

        /// <summary>
        /// asTable.
        /// </summary>
        protected string asTableNamePoint;

        /// <summary>
        /// format格式化后增加的长度
        /// </summary>
        protected int formatColumnAppendCount;

        /// <summary>
        /// 等于的前缀
        /// </summary>
        protected string equalAndPrefix;

        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public UpdatingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public override int GetResult()
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "update")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Update(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text) 
        {
            return text;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        protected override string FormatColumn(string text)
        {
            return text;
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override UpdateContext<Parameter> StartSetColumn()
        {
            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.FromTable, ".");
            this.asTableNamePoint = this.AsTable.IsNullOrEmpty() ? string.Empty : string.Concat(this.AsTable, ".");

            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.FromTable, "\r") };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        protected virtual UpdateContext<Parameter> SetColum<TMember>(string columnName, bool textParameter)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            var selectTableName = this.SelectTableNamePointOnSetolunm() ?? this.tableNamePoint;

            if (setTimes == 0)
            {
                setTimes++;
                label.SqlText = string.Concat("set ", selectTableName, this.FormatColumn(columnName), equalAndPrefix, columnName, " \r");
                label.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = columnName,
                    OccupanLength = this.formatColumnAppendCount + columnName.Length,
                    PrefixStartIndex = 4 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                    ParameterStartIndex = 4 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                    ParameterStopIndex = 4 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
                    TextParameter = textParameter,
                });
            }
            else
            {
                label.SqlText = string.Concat(",", selectTableName, this.FormatColumn(columnName), equalAndPrefix, columnName, " \r");
                label.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = columnName,
                    OccupanLength = this.formatColumnAppendCount + columnName.Length,
                    PrefixStartIndex = 1 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                    ParameterStartIndex = 1 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                    ParameterStopIndex = 1 + selectTableName.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
                    TextParameter = textParameter,
                });
            }

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;

            return this;
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            return this.SetColum<TMember>(columnName, false);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColum<TMember>(columnName, true);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColum<TMember>(columnName, false);
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where()
        {
            var label = new TextLabel()
            {
                SqlText = "where 1 = 1 \r",
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> End(string sql)
        {
            if (sql.IsNullOrEmpty())
                return this;

            var label = new TextLabel()
            {
                SqlText = sql,
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where ", this.asTableNamePoint, this.FormatColumn(columnName), this.equalAndPrefix, columnName, "\r"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                OccupanLength = this.formatColumnAppendCount + columnName.Length,
                PrefixStartIndex = 6 + this.asTableNamePoint.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                ParameterStartIndex = 6 + this.asTableNamePoint.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length,
                ParameterStopIndex = 6 + this.asTableNamePoint.Length + this.formatColumnAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
                TextParameter = false,
            });

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSetolunm()
        {
            return this.asTableNamePoint;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> JoinOnUpdate(List<JoinInfo> joins)
        {
            var label = new TextLabel()
            {
                SqlText = this.LoadUpdateJoin(this.FromTable, this.AsTable, joins).ToString(),
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> JoinOnWhereExists(WhereExists whereExists)
        {
            var label = new TextLabel()
            {
                SqlText = this.LoadWhereExists(this.FromTable, this.AsTable, whereExists).ToString(),
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> JoinOnWhereIn(WhereIn whereIn)
        {
            var label = new TextLabel()
            {
                SqlText = this.LoadWhereIn(this.FromTable, this.AsTable, whereIn).ToString(),
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// where sql
        /// </summary>
        /// <param name="andOrOption"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> Where(AndOrOption andOrOption, string sql)
        {
            var label = new TextLabel()
            {
                SqlText = string.Concat(andOrOption == AndOrOption.and ? "and " : "or ", sql),
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }
    }
}
