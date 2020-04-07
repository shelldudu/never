using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpdatedContext<Parameter> : Linq.UpdateContext<Parameter>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly LinqSqlTag sqlTag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public UpdatedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override void AsTable(string table)
        {
            return;
        }

        public override Linq.UpdateContext<Parameter> Exists<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override Linq.UpdateContext<Parameter> In<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> In(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotExists<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotIn<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColum<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.templateParameter.Add(this.FindColumnName(expression, this.tableInfo, out _), value);
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, object value)
        {
            this.templateParameter.Add(this.FindColumnName(expression, this.tableInfo, out _), value);
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
