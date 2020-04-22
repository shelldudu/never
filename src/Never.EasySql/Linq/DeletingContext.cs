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
    /// 删除操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public class DeletingContext<Parameter> : DeleteContext<Parameter>
    {
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
        /// tablaName
        /// </summary>
        protected string tableName;
        /// <summary>
        /// as别名
        /// </summary>
        protected string asTableName;
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
        protected int formatAppendCount;
        /// <summary>
        /// 等于的前缀
        /// </summary>
        protected string equalAndPrefix;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public DeletingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public override Linq.DeleteContext<Parameter> From(string table)
        {
            this.tableName = table;
            return this;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public override Linq.DeleteContext<Parameter> AsTable(string table)
        {
            this.asTableName = table;
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public override int GetResult()
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "delete")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Execute(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override Linq.DeleteContext<Parameter> Entrance()
        {
            this.tableName = this.tableName.IsNullOrEmpty() ? this.FindTableName<Parameter>(tableInfo) : this.tableName;
            int length = this.tableName.Length;
            this.tableName = this.Format(this.tableName);
            this.formatAppendCount = this.tableName.Length - length;

            if (this.asTableName.IsNullOrEmpty())
            {
                var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("delete ", this.tableName, "\r") };
                this.textLength += label.SqlText.Length;
                this.labels.Add(label);
            }
            else
            {
                var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("delete ", asTableName, " from ", this.tableName, " ", "\r") };
                this.textLength += label.SqlText.Length;
                this.labels.Add(label);
            }

            this.tableNamePoint = string.Concat(this.tableName, ".");
            this.asTableNamePoint = this.asTableName.IsNullOrEmpty() ? string.Empty : string.Concat(this.asTableName, ".");
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;

        }


        /// <summary>
        /// where 条件
        /// </summary>
        public override Linq.DeleteContext<Parameter> Where()
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
        /// where 条件
        /// </summary>
        public override Linq.DeleteContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where ", this.asTableNamePoint, this.Format(columnName), this.equalAndPrefix, columnName, "\r"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                OccupanLength = this.formatAppendCount + columnName.Length,
                PrefixStartIndex = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                ParameterStartIndex = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                ParameterStopIndex = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
                TextParameter = false,
            });

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// in
        /// </summary>
        public override DeleteContext<Parameter> In(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// int
        /// </summary>
        public override DeleteContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        /// <summary>
        /// not in
        /// </summary>
        public override DeleteContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// not in
        /// </summary>
        public override DeleteContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        public override DeleteContext<Parameter> Exists(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        public override DeleteContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        /// <summary>
        /// not exists
        /// </summary>
        public override DeleteContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// not exists
        /// </summary>
        public override DeleteContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string Format(string text)
        {
            return text;
        }
    }
}
