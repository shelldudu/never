using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class SelectedContext<Table,Parameter> : SelectContext<Table,Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public SelectedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override SelectContext<Table,Parameter> Append(string sql)
        {
            return this;
        }
        public override SelectContext<Table,Parameter> Last(string sql)
        {
            return this;
        }
        public override Table GetResult()
        {
            return this.Select<Table,Parameter>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            this.templateParameter["StartIndex"] = startIndex;
            this.templateParameter["EndIndex"] = endIndex;
            return this.SelectMany<Table,Parameter>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }


        public override SelectContext<Table,Parameter> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        public override SelectContext<Table,Parameter> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return this;
        }

        protected override SelectContext<Table,Parameter> SelectColumn(string column, string originalColunmName, string @as)
        {
            return this;
        }

        public override SelectContext<Table,Parameter> SelectAll()
        {
            return this;
        }

        public override SelectContext<Table,Parameter> StartSelectColumn()
        {
            return this;
        }

        public override SelectContext<Table,Parameter> Where()
        {
            return this;
        }

        public override SelectContext<Table,Parameter> Where(Expression<Func<Table,Parameter, bool>> expression)
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

        public override SelectContext<Table,Parameter> AppenInWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override SelectContext<Table,Parameter> AppenInWhereIn(WhereInInfo whereExists)
        {
            return this;
        }
    }
}
