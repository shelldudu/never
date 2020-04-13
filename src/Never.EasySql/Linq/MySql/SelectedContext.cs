using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public sealed class SelectedContext<Parameter, Table> : Linq.SelectContext<Parameter, Table>
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
        public SelectedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override void AsTable(string table)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Entrance()
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Exists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override void From(string table)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> In<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotExists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotIn<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotIn(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Where()
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
        {
            throw new NotImplementedException();
        }

        protected override string Format(string text)
        {
            throw new NotImplementedException();
        }
    }
}
