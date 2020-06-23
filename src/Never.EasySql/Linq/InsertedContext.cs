using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class InsertedContext<Parameter, Table> : Linq.InsertContext<Parameter, Table>
    {
        private readonly LinqSqlTag sqlTag;

        public InsertedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> expression)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> expression, string function)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> Entrance(char flag)
        {
            return this;
        }

        public override Result GetResult<Result>()
        {
            throw new NotImplementedException();
        }

        public override void GetResult()
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter, Table> InsertLastInsertId()
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> Into(string table)
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
