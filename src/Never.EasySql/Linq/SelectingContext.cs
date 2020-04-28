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
    /// select语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public class SelectingContext<Parameter, Table> : SelectContext<Parameter, Table>
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
        /// select出现的的次数
        /// </summary>
        protected int selectTimes;

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

        /// <summary>
        /// select join
        /// </summary>
        protected List<JoinInfo> selectJoin;

        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public SelectingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public override Table GetResult()
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Select<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="paged"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(PagedSearch paged)
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            if ((this.sqlParameter.Object is PagedSearch) == false)
            {
                if (paged == null)
                    paged = new PagedSearch();

                this.templateParameter["PageNow"] = paged.PageNow;
                this.templateParameter["PageSize"] = paged.PageSize;
                this.templateParameter["StartIndex"] = paged.StartIndex;
                this.templateParameter["EndIndex"] = paged.EndIndex;
                if (paged.BeginTime.HasValue)
                    this.templateParameter["BeginTime"] = paged.BeginTime.Value;
                if (paged.EndTime.HasValue)
                    this.templateParameter["EndTime"] = paged.EndTime.Value;

            }


            LinqSqlTagProvider.Set(sqlTag);
            return this.SelectMany<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 检查名字
        /// </summary>
        /// <param name="tableName"></param>
        public override void CheckTableNameIsExists(string tableName)
        {
            base.CheckTableNameIsExists(tableName);
            if (this.selectJoin != null && this.selectJoin.Any())
            {
                foreach (var a in this.selectJoin)
                {
                    if (a.AsName.IsEquals(tableName, StringComparison.OrdinalIgnoreCase))
                        throw new Exception(string.Format("the table alias name {0} is equal alias Name {1}", a.AsName, tableName));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> JoinOnSelect(List<JoinInfo> joins)
        {
            this.selectJoin = joins;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> JoinOnWhereExists(WhereExists whereExists)
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
        /// 
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> JoinOnWhereIn(WhereIn whereIn)
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
        /// 查询字段
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="originalColunmName"></param>
        /// <param name="as"></param>
        /// <returns></returns>
        protected override SelectContext<Parameter, Table> SelectColumn(string memberName, string originalColunmName, string @as)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (selectTimes == 0)
            {
                selectTimes++;
                label.SqlText = string.Concat(" ", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r");
            }
            else
            {
                label.SqlText = string.Concat(",", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r");
            }

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> SelectAll()
        {
            if (this.selectTimes > 0)
                throw new Exception("select * just use one time");

            return base.SelectAll();
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override SelectContext<Parameter, Table> StartSelectColumn()
        {
            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.FromTable, ".");
            this.asTableNamePoint = this.AsTable.IsNullOrEmpty() ? string.Empty : string.Concat(this.AsTable, ".");

            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = "select " };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSelectColunm()
        {
            return this.asTableNamePoint.IsNullOrEmpty() ? this.tableNamePoint : this.asTableNamePoint;
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where()
        {
            if (this.selectJoin.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = this.LoadUpdateJoin(this.FromTable, this.AsTable, selectJoin).ToString(),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            var label2 = new TextLabel()
            {
                SqlText = "where 1 = 1 \r",
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label2);
            this.textLength += label2.SqlText.Length;
            return this;
        }

        /// <summary>
        /// where
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
        {
            if (this.AsTable.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = string.Concat("from ", this.FromTable, " as ", this.AsTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r"),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            if (this.selectJoin.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = this.LoadUpdateJoin(this.FromTable, this.AsTable, selectJoin).ToString(),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            var label2 = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where ", this.asTableNamePoint, this.FormatColumn(columnName), this.equalAndPrefix, columnName, "\r"),
            };

            label2.Add(new SqlTagParameterPosition()
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

            this.labels.Add(label2);
            this.textLength += label2.SqlText.Length;
            return this;
        }

        /// <summary>
        /// where sql
        /// </summary>
        /// <param name="andOrOption"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(AndOrOption andOrOption, string sql)
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
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Append(string sql)
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
    }
}
