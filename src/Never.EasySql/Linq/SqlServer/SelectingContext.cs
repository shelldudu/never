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
        public override SelectContext<Parameter, Table> StartSelectColumn()
        {
            if (this.isSingle)
                return base.StartSelectColumn();

            if (this.orderBies.IsNullOrEmpty())
            {
                var primary = this.tableInfo.Columns.Where(ta => ta.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.Primary);
                if (primary.IsNullOrEmpty())
                    primary = this.tableInfo.Columns.Where(ta => ta.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement);

                if (this.tableInfo.Columns.Any(ta => ta.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.Primary))
                {
                    this.orderBies.Add(new OrderByInfo
                    {
                        Flag = string.Concat("order by ", primary.FirstOrDefault().Column.Alias.IsNullOrEmpty() ? primary.FirstOrDefault().Member.Name : primary.FirstOrDefault().Column.Alias, " desc"),
                        Placeholder = this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable,
                        Type = typeof(Table)
                    });
                }
            }

            return base.StartSelectColumn();
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where()
        {
            if (this.orderBies.IsNotNullOrEmpty() || this.selectJoin.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = this.LoadJoin(this.FromTable, this.AsTable, selectJoin).ToString(),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return base.Where();
        }

        /// <summary>
        /// wehre
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
        {
            return base.Where(expression);
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
        /// <param name="paged"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(PagedSearch paged)
        {
            return base.GetResults(paged);
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
