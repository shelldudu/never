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
                SqlText = ") as qwertyuiop",
            };
            this.labels.Add(label);

            label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where", " qwertyuiop._ >= ", this.dao.SqlExecuter.GetParameterPrefix(), "StartIndex", " and qwertyuiop._ < ", this.dao.SqlExecuter.GetParameterPrefix(), "EndIndex"),
            };
            this.labels.Add(label);

            return base.GetResults(startIndex, endIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnWhereInit()
        {
            if (this.isSingle)
            {
                base.OnWhereInit();
                return;
            }

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

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(", ", (this.LoadOrderBy(this.orderBies).ToString()), " as _"),
            };

            this.labels.Add(label);
            this.orderBies.Clear();
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
