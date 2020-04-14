using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class DeletedContext<Parameter> : _DeleteContext<Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq._DeleteContext<Parameter> AsTable(string table)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> Entrance()
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> From(string table)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override Linq._DeleteContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> Where()
        {
            return this;
        }

        public override Linq._DeleteContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        protected override string Format(string text)
        {
            return text;
        }
    }
}
