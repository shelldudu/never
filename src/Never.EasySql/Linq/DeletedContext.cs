using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class DeletedContext<Parameter, Table> : DeleteContext<Parameter,Table>
    {
        private readonly LinqSqlTag sqlTag;

        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.DeleteContext<Parameter, Table> AsTable(string table)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> Entrance()
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> Exists<Table1>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> From(string table)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override Linq.DeleteContext<Parameter, Table> In<Table1>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> NotExists<Table1>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> NotIn<Table1>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table1, bool>> where)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> NotIn(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> Where()
        {
            return this;
        }

        public override Linq.DeleteContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
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
