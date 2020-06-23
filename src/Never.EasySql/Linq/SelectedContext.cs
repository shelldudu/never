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
        public override SelectContext<Parameter, Table> Last(string sql)
        {
            return this;
        }
        public override Table GetResult()
        {
            return this.Select<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            this.templateParameter["StartIndex"] = startIndex;
            this.templateParameter["EndIndex"] = endIndex;
            return this.SelectMany<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
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

        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, Table, object>> expression)
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

        public override SelectContext<Parameter, Table> AppenInWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> AppenInWhereIn(WhereInInfo whereExists)
        {
            return this;
        }
    }
}
