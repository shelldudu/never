using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class DeletedContext<Table,Parameter> : DeleteContext<Table, Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override DeleteContext<Table,Parameter> AsTable(string table)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> Entrance()
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> Exists<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> From(string table)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override Linq.DeleteContext<Table,Parameter> In<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> NotExists<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> NotIn<Table1>(AndOrOption option, Expression<Func<Table,Parameter, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> NotIn(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> Where()
        {
            return this;
        }

        public override Linq.DeleteContext<Table,Parameter> Where(Expression<Func<Parameter, object>> expression)
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
