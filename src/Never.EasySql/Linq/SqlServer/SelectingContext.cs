using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.SqlServer
{
    /// <summary>
    /// 查询操作
    /// </summary>
    public sealed class SelectingContext<Parameter, Table> : Linq.SelectingContext<Parameter, Table>
    {
        /// <summary>
        /// 分页的配置
        /// </summary>
        private TextLabel rowNumberTextLabel;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public SelectingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(cacheId, dao, tableInfo, sqlParameter)
        {
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> StartEntrance()
        {
            return base.StartEntrance();
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where()
        {
            return base.Where();
        }

        /// <summary>
        /// wehre
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="andOr"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            return base.Where(expression, andOr);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> SetPage()
        {
            this.rowNumberTextLabel = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            return base.SetPage();
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override Table GetResult()
        {
            return base.GetResult();
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = "select qwertyuiop.* from (",
            };
            this.labels.Insert(0, label);

            label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = ") as qwertyuiop\r",
            };
            this.labels.Add(label);

            label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where", " qwertyuiop._ >= ", this.dao.SqlExecuter.GetParameterPrefix(), "StartIndex", " and qwertyuiop._ <= ", this.dao.SqlExecuter.GetParameterPrefix(), "EndIndex"),
            };


            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "StartIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length,
                PrefixStartIndex = "where qwertyuiop._ >= ".Length,
                ParameterStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length - 1,
                TextParameter = false,
            });
            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "EndIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length,
                PrefixStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length,
                ParameterStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length - 1,
                TextParameter = false,
            });

            this.labels.Add(label);

            if (this.isSingle)
            {
                if (this.orderBies.IsNotNullOrEmpty())
                    base.LoadOrderBy(true);
            }
            else
            {
                this.LoadRowNumber();
            }
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
        /// 加载分页
        /// </summary>
        /// <param name="clear"></param>
        public void LoadRowNumber(bool clear = true)
        {
            if (this.onWhereInited == false && this.rowNumberTextLabel != null)
                this.labels.Add(this.rowNumberTextLabel);

            if (this.orderBies.IsNullOrEmpty())
            {
                var primary = this.tableInfo.Columns.Where(ta => ta.Column != null && ta.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.Primary);
                if (primary.IsNullOrEmpty())
                {
                    primary = this.tableInfo.Columns.Where(ta => ta.Column != null && ta.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement);
                }
                if (primary.IsNullOrEmpty())
                {
                    primary = this.tableInfo.Columns;

                }

                if (primary.Any())
                {
                    this.orderBies.Add(new OrderByInfo
                    {
                        Flag = string.Concat("order by ", primary.FirstOrDefault().Column.Alias.IsNullOrEmpty() ? primary.FirstOrDefault().Member.Name : primary.FirstOrDefault().Column.Alias, " desc"),
                        Placeholder = this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable,
                        Type = typeof(Table)
                    });
                }
            }

            this.rowNumberTextLabel.SqlText = string.Concat(", row_number() over (", this.LoadOrderByContent(this.orderBies, this.dao).ToString(), ") as _\r");
            this.textLength += this.rowNumberTextLabel.SqlText.Length;

            if (clear)
                this.orderBies.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatText"></param>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = "select qwertyuiop.* from (",
            };
            this.labels.Insert(0, label);

            label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = ") as qwertyuiop \r",
            };
            this.labels.Add(label);

            label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where", " qwertyuiop._ >= ", this.dao.SqlExecuter.GetParameterPrefix(), "StartIndex", " and qwertyuiop._ <= ", this.dao.SqlExecuter.GetParameterPrefix(), "EndIndex"),
            };


            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "StartIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length,
                PrefixStartIndex = "where qwertyuiop._ >= ".Length,
                ParameterStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length - 1,
                TextParameter = false,
            });
            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "EndIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length,
                PrefixStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length,
                ParameterStartIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "where qwertyuiop._ >= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex and qwertyuiop._ <= ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length - 1,
                TextParameter = false,
            });

            this.labels.Add(label);

            if (this.isSingle)
            {
                if (this.orderBies.IsNotNullOrEmpty())
                    base.LoadOrderBy(true);
            }
            else
            {
                this.LoadRowNumber();
            }
            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            int StartIndex = 0, EndIndex = 1;
            this.templateParameter["StartIndex"] = StartIndex;
            this.templateParameter["EndIndex"] = EndIndex;

            return this.dao.GetSqlTagFormat<Parameter>(sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnWhereInit()
        {
            base.OnWhereInit();

            if (this.rowNumberTextLabel != null)
                this.labels.Add(this.rowNumberTextLabel);
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text)
        {
            return string.Concat("[", text, "]");
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatColumn(string text)
        {
            return string.Concat("[", text, "]");
        }
    }
}
