using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class InsertedContext<Parameter> : Linq.InsertContext<Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public InsertedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.InsertContext<Parameter> Colum(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter> ColumWithFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter> ColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter> Entrance(char flag)
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

        public override Linq.InsertContext<Parameter> InsertLastInsertId()
        {
            return this;
        }

        public override Linq.InsertContext<Parameter> Into(string table)
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
