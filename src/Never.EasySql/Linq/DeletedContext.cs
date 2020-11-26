using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    internal sealed class DeletedContext<Parameter, Table> : DeleteContext<Parameter, Table>
    {
        private readonly LinqSqlTag sqlTag;

        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override DeleteContext<Parameter, Table> Then(string sql)
        {
            return this;
        }

        public override int GetResult()
        {
            return this.Delete<Parameter, Table>(this.sqlTag, this.dao, this.sqlParameter);
        }

        public override DeleteContext<Parameter, Table> JoinOnDelete(List<JoinInfo> joins)
        {
            return this;
        }

        public override DeleteContext<Parameter, Table> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override DeleteContext<Parameter, Table> JoinOnWhereIn(WhereInInfo whereIn)
        {
            return this;
        }

        public override DeleteContext<Parameter, Table> StartEntrance()
        {
            return this;
        }

        public override DeleteContext<Parameter, Table> Where()
        {
            return this;
        }

        public override DeleteContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
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

        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            return this.dao.GetSqlTagFormat<Parameter>(this.sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }
    }
}
