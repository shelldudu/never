using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class SelectedContext<Parameter, Table> : SelectContext<Parameter, Table>
    {
        private readonly LinqSqlTag sqlTag;

        public SelectedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override SelectContext<Parameter, Table> Append(string sql)
        {
            return this;
        }

        public override Table GetResult()
        {
            return this.Select<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override IEnumerable<Table> GetResults(PagedSearch paged)
        {
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

            return this.SelectMany<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override SelectContext<Parameter, Table> JoinOnSelect(List<JoinInfo> joins)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> JoinOnWhereExists(WhereExists whereExists)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> JoinOnWhereIn(WhereIn whereIn)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return this;
        }

        protected override SelectContext<Parameter, Table> SelectColumn(string column, string originalColunmName, string @as)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> SelectAll()
        {
            return this;
        }

        public override SelectContext<Parameter, Table> StartSelectColumn()
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Where()
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Where(AndOrOption andOrOption, string sql)
        {
            return this;
        }

        protected override string FormatColumn(string text)
        {
            return text;
        }

        protected override string FormatTable(string text)
        {
            return text;
        }

        protected override string SelectTableNamePointOnSelectColunm()
        {
            return string.Empty;
        }
    }
}
