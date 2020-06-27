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

        public override Linq.InsertContext<Table,Parameter> Colum(Expression<Func<Table, object>> keyValue)
        {
            return this;
        }

        public override InsertContext<Table, Parameter> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            return this;
        }

        public override Linq.InsertContext<Table,Parameter> StartInsertColumn(char flag)
        {
            return this;
        }

        public override Result GetResult<Result>()
        {
            return this.Insert<Table, Parameter,Result>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override void GetResult()
        {
            this.InsertMany<Table, Parameter>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
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
