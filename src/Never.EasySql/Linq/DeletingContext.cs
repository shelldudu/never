﻿using Never.EasySql.Labels;
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
    /// 删除操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public class DeletingContext<Parameter, Table> : DeleteContext<Parameter, Table>
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
        public DeletingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
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
            return this.Delete<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "delete")
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
        public override DeleteContext<Parameter, Table> StartEntrance()
        {
            if (this.FromTable.IsNullOrEmpty())
            {
                this.From(this.FindTableName(this.tableInfo, typeof(Table)));
            }

            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.FromTable, ".");
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber()
            };

            if (this.AsTable.IsNullOrEmpty())
            {
                this.asTableNamePoint = string.Empty;
                label.SqlText = string.Concat("delete ", this.FromTable, "\rfrom", this.FromTable, "\r");
                this.textLength += label.SqlText.Length;
            }
            else
            {
                this.asTableNamePoint = string.Concat(this.AsTable, ".");
                label.SqlText = string.Concat("delete ", this.AsTable, " from ", this.FromTable, " as ", this.AsTable, "\r");
                this.textLength += label.SqlText.Length;
            }

            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DeleteContext<Parameter, Table> Where()
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
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="andOr"></param>
        /// <returns></returns>
        public override DeleteContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (whereCount == 0)
            {
                whereCount++;
                label.SqlText = string.Concat("where ");
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
            }
            else
            {
                label.SqlText = andOr == null ? "and " : string.Concat(andOr, " ");
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
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DeleteContext<Parameter, Table> AddSql(string sql)
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
        /// <param name="joins"></param>
        /// <returns></returns>
        public override DeleteContext<Parameter, Table> JoinOnDelete(List<JoinInfo> joins)
        {
            this.updateJoin = joins;
            var label = new TextLabel()
            {
                SqlText = this.LoadJoin(this.FromTable, this.AsTable, joins).ToString(),
                TagId = NewId.GenerateNumber(),
            };

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public override DeleteContext<Parameter, Table> JoinOnWhereExists(WhereExistsInfo whereExists)
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
        public override DeleteContext<Parameter, Table> JoinOnWhereIn(WhereInInfo whereIn)
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
    }
}
