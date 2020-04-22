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

        public override SelectContext<Parameter, Table> AsTable(string table)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Entrance()
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Exists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> From(string table)
        {
            return this;
        }

        public override Table GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override IEnumerable<Table> GetResults()
        {
            return this.Execute2(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override SelectContext<Parameter, Table> In<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> NotExists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> NotIn<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            return this;
        }

        public override SelectContext<Parameter, Table> NotIn(AndOrOption option, string expression)
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

        protected override string FormatColumn(string text)
        {
            return text;
        }

        protected override string FormatTable(string text)
        {
            return text;
        }
    }
}
