using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class DeletedContext<Table,Parameter> : DeleteContext<Table, Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override DeleteContext<Table, Parameter> Append(string sql)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Delete<Table, Parameter>(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override DeleteContext<Table, Parameter> JoinOnDelete(List<JoinInfo> joins)
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> JoinOnWhereIn(WhereInInfo whereIn)
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> StartEntrance()
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> Where()
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> Where(Expression<Func<Table, Parameter, bool>> expression)
        {
            return this;
        }

        public override DeleteContext<Table, Parameter> Where(AndOrOption andOrOption, string sql)
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
