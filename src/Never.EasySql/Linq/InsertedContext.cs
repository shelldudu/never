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

        public override Linq.InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> keyValue)
        {
            return this;
        }

        public override InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> StartEntrance()
        {
            return this;
        }

        public override Result GetResult<Result>()
        {
            return this.Insert<Table, Parameter, Result>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override void GetResult()
        {
            this.Insert<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override Linq.InsertContext<Parameter, Table> InsertLastInsertId<ReturnType>()
        {
            return this;
        }

        public override Linq.InsertContext<Parameter, Table> Into(string table)
        {
            return this;
        }

        protected override string FormatTableAndColumn(string text)
        {
            return text;
        }

        public override InsertContext<Parameter, Table> InsertColumn(string columnName, string parameterName, bool textParameter, bool function)
        {
            return this;
        }

        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            return this.dao.GetSqlTagFormat<Parameter>(this.sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }
    }
}
