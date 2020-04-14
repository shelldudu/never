using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class UpdatedContext<Parameter> : Linq.UpdateContext<Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public UpdatedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.UpdateContext<Parameter> AsTable(string table)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Entrance()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> From(string table)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override Linq.UpdateContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.templateParameter[this.FindColumnName(expression, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.templateParameter[this.FindColumnName(expression, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter> Where()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        protected override string Format(string text)
        {
            return text;
        }
    }
}
