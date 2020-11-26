using Never.EasySql.Labels;
using Never.Serialization.Json;
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
    public class UpdatingContext<Parameter, Table> : UpdateContext<Parameter, Table>
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

        /// <summary>
        /// update join
        /// </summary>
        protected List<JoinInfo> updateJoin;

        /// <summary>
        /// where的条数
        /// </summary>
        private int whereCount = 0;
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
            return this.Update<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "update")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            return this.dao.GetSqlTagFormat<Parameter>(sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
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
        /// 检查名字
        /// </summary>
        /// <param name="tableName"></param>
        public override void CheckTableNameIsExists(string tableName)
        {
            base.CheckTableNameIsExists(tableName);
            if (this.updateJoin != null && this.updateJoin.Any())
            {
                foreach (var a in this.updateJoin)
                {
                    if (a.AsName.IsEquals(tableName, StringComparison.OrdinalIgnoreCase))
                        throw new Exception(string.Format("the table alias name {0} is equal alias Name {1}", a.AsName, tableName));
                }
            }
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override UpdateContext<Parameter, Table> StartEntrance()
        {
            if (this.FromTable.IsNullOrEmpty())
            {
                this.From(this.FindTableName(this.tableInfo, typeof(Table)));
            }

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
        public override UpdateContext<Parameter, Table> SetColumn(string columnName, string parameterName, bool textParameter, bool function)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (textParameter == false)
            {
                if (setTimes == 0)
                {
                    setTimes++;
                    label.SqlText = string.Concat("set ", string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), this.equalAndPrefix, parameterName, " \r");
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + columnName.Length,
                        PrefixStartIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length - 1,
                        ParameterStartIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length,
                        ParameterStopIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
                else
                {
                    label.SqlText = string.Concat(",", string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), this.equalAndPrefix, parameterName, " \r");
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + columnName.Length,
                        PrefixStartIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length - 1,
                        ParameterStartIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length,
                        ParameterStopIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + this.equalAndPrefix.Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
            }
            else
            {
                var equalAndQuotation = function ? " = " : " = '";
                var equalAndQuotationEnd = function ? " \r" : "' \r";
                if (setTimes == 0)
                {
                    setTimes++;
                    label.SqlText = string.Concat("set ", string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), equalAndQuotation, this.dao.SqlExecuter.GetParameterPrefix(), parameterName, equalAndQuotationEnd);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = columnName.Length + 1,
                        PrefixStartIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length,
                        ParameterStartIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = 4 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
                else
                {
                    label.SqlText = string.Concat(",", string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)), equalAndQuotation, this.dao.SqlExecuter.GetParameterPrefix(), parameterName, equalAndQuotationEnd);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = columnName.Length + 1,
                        PrefixStartIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length,
                        ParameterStartIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = 1 + string.Concat(this.SelectTableNamePointOnSetColunm(), this.FormatColumn(columnName)).Length + equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
            }



            this.labels.Add(label);
            this.textLength += label.SqlText.Length;

            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter, Table> Where()
        {
            if (whereCount == 0)
            {
                whereCount++;
                var label = new TextLabel()
                {
                    SqlText = "where 1 = 1 \r",
                    TagId = NewId.GenerateNumber(),
                };
                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return this;
        }


        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (whereCount == 0)
            {
                whereCount++;
                label.SqlText = string.Concat("where ");
            }
            else
            {
                label.SqlText = andOr == null ? "and " : string.Concat(andOr, " ");
            }

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            int s = this.labels.Count;
            if (this.AnalyzeWhereExpress(expression, this.labels, this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable, this.dao.SqlExecuter.GetParameterPrefix()))
            {
                int e = this.labels.Count;
                for (var i = s; i < e; i++)
                {
                    this.textLength += this.labels[i].SqlText.Length;
                }

                label = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = "\r",
                };
                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return this;
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter, Table> AddSql(string sql)
        {
            if (sql.IsNullOrEmpty())
                return this;


            var label = new SqlTag().ReadTextNodeUsingFormatLine(sql, new ThunderWriter(sql.Length), new SequenceStringReader("1"), this.dao.SqlExecuter.GetParameterPrefix(), true);
            label.TagId = NewId.GenerateNumber();
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string SelectTableNamePointOnSetColunm()
        {
            return this.asTableNamePoint.IsNullOrEmpty() ? this.tableNamePoint : this.asTableNamePoint;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter, Table> JoinOnUpdate(List<JoinInfo> joins)
        {
            this.updateJoin = joins;
            var label = this.LoadJoinLabel(this.FromTable, this.AsTable, joins, this.dao);
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter, Table> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            var label = this.LoadWhereExistsLabel(this.FromTable, this.AsTable, whereExists, this.dao);
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// in
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter, Table> JoinOnWhereIn(WhereInInfo whereIn)
        {
            var label = this.LoadWhereInLabel(this.FromTable, this.AsTable, whereIn, this.dao);

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }
    }
}
