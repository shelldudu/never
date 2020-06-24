using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class InsertedContext<Table,Parameter> : Linq.InsertContext<Table,Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public InsertedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.InsertContext<Table,Parameter> Colum(Expression<Func<Table, object>> expression)
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> ColumWithFunc(Expression<Func<Table, object>> expression, string function)
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> ColumWithValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value)
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> Entrance(char flag)
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

        public override Linq.InsertContext<Table,Parameter> InsertLastInsertId()
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> Into(string table)
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
