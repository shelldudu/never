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
        /// where的调用方法第一次
        /// </summary>
        protected bool onWhereInited;

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
            this.LoadOrderBy(true);
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
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            this.LoadOrderBy(true);
            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            this.templateParameter["StartIndex"] = startIndex;
            this.templateParameter["EndIndex"] = endIndex;

            LinqSqlTagProvider.Set(sqlTag);
            return this.SelectMany<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            this.LoadOrderBy(true);
            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            if (this.isSingle == false)
            {
                this.templateParameter["StartIndex"] = 0;
                this.templateParameter["EndIndex"] = 1;
            }

            return this.dao.GetSqlTagFormat<Parameter>(sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }

        /// <summary>
        /// 加载orderby
        /// </summary>
        /// <param name="clear"></param>
        protected void LoadOrderBy(bool clear = true)
        {
            if (this.orderBies.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = "\r",
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;

                label = this.LoadOrderByLabel(this.orderBies, this.dao);
                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            if (clear)
                this.orderBies.Clear();
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
        public override SelectContext<Parameter, Table> StartEntrance()
        {
            if (this.FromTable.IsNullOrEmpty())
            {
                this.From(this.FindTableName(this.tableInfo, typeof(Table)));
            }

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
            if (this.onWhereInited == false)
            {
                this.onWhereInited = true;
                this.OnWhereInit();
            }

            if (whereCount == 0)
            {
                whereCount++;
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

                else
                {
                    var label = new TextLabel()
                    {
                        SqlText = string.Concat("from ", this.FromTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r"),
                        TagId = NewId.GenerateNumber(),
                    };

                    this.labels.Add(label);
                    this.textLength += label.SqlText.Length;
                }


                if (this.selectJoin.IsNotNullOrEmpty())
                {
                    var label = this.LoadJoinLabel(this.FromTable, this.AsTable, selectJoin, this.dao);
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
            }

            return this;
        }

        /// <summary>
        /// where
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="andOr"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            if (this.onWhereInited == false)
            {
                this.onWhereInited = true;
                this.OnWhereInit();
            }

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (whereCount == 0)
            {
                whereCount++;
                label.SqlText = string.Concat("where ");

                if (this.AsTable.IsNotNullOrEmpty())
                {
                    var label2 = new TextLabel()
                    {
                        SqlText = string.Concat("from ", this.FromTable, " as ", this.AsTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r"),
                        TagId = NewId.GenerateNumber(),
                    };

                    this.labels.Add(label2);
                    this.textLength += label.SqlText.Length;
                }
                else
                {
                    var label2 = new TextLabel()
                    {
                        SqlText = string.Concat("from ", this.FromTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r"),
                        TagId = NewId.GenerateNumber(),
                    };

                    this.labels.Add(label2);
                    this.textLength += label.SqlText.Length;
                }

                if (this.selectJoin.IsNotNullOrEmpty())
                {
                    var label2 = this.LoadJoinLabel(this.FromTable, this.AsTable, selectJoin, this.dao);
                    this.labels.Add(label2);
                    this.textLength += label.SqlText.Length;
                }

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
                int s = this.labels.Count;
                if (base.AnalyzeWhereExpress(expression, this.labels, this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable, this.dao.SqlExecuter.GetParameterPrefix()))
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
                if (base.AnalyzeWhereExpress(expression, this.labels, this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable, this.dao.SqlExecuter.GetParameterPrefix()))
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
        /// on where init
        /// </summary>
        protected virtual void OnWhereInit()
        {
            if (this.selectTimes == 0)
                this.SelectAll();
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
        public override SelectContext<Parameter, Table> AddSql(string sql)
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
        /// <param name="whereExists"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> AddInWhereExists(WhereExistsInfo whereExists)
        {

            var label = this.LoadWhereExistsLabel(this.FromTable, this.AsTable, whereExists, this.dao);
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> AddInWhereIn(WhereInInfo whereIn)
        {
            var label = this.LoadWhereInLabel(this.FromTable, this.AsTable, whereIn, this.dao);
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }
    }
}
