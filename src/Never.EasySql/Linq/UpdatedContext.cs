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
    internal sealed class UpdatedContext<Parameter, Table> : Linq.UpdateContext<Parameter, Table>
    {
        private readonly LinqSqlTag sqlTag;

        public UpdatedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.UpdateContext<Parameter, Table> As(string table)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> StartEntrance()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> From(string table)
        {
            return this;
        }

        public override void CheckTableNameIsExists(string tableName)
        {
        }

        protected override string ClearThenFormatColumn(string text)
        {
            return text;
        }

        public override int GetResult()
        {
            return this.Update<Parameter, Table>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override Linq.UpdateContext<Parameter, Table> Set<TMember>(Expression<Func<Table, TMember>> key, Expression<Func<Parameter, TMember>> value)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> SetFunc<TMember>(Expression<Func<Table, TMember>> key, string value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> SetValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> Where()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            return this;
        }

        public override UpdateContext<Parameter, Table> JoinOnUpdate(List<JoinInfo> joins)
        {
            return this;
        }

        public override UpdateContext<Parameter, Table> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override UpdateContext<Parameter, Table> JoinOnWhereIn(WhereInInfo whereIn)
        {
            return this;
        }

        public override UpdateContext<Parameter, Table> SetColumn(string columnName, string parameterName, bool textParameter, bool function)
        {
            return this;
        }

        public override UpdateContext<Parameter, Table> Then(string sql)
        {
            return this;
        }

        protected override string FormatTable(string text)
        {
            return string.Empty;
        }

        protected override string FormatColumn(string text)
        {
            return string.Empty;
        }

        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            return this.dao.GetSqlTagFormat<Parameter>(this.sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }
    }
}
