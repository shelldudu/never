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
    internal sealed class UpdatedContext<Table, Parameter> : Linq.UpdateContext<Table, Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public UpdatedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.UpdateContext<Table, Parameter> As(string table)
        {
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> StartEntrance()
        {
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> From(string table)
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
            return this.Update<Table, Parameter>(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override Linq.UpdateContext<Table, Parameter> Set<TMember>(Expression<Func<Table, TMember>> key, Expression<Func<Parameter, TMember>> value)
        {
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> SetFunc<TMember>(Expression<Func<Table, TMember>> key, string value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> SetValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.templateParameter[this.FindColumnName(key, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> Where()
        {
            return this;
        }

        public override Linq.UpdateContext<Table, Parameter> Where(Expression<Func<Table, Parameter, bool>> expression)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> JoinOnUpdate(List<JoinInfo> joins)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> JoinOnWhereIn(WhereInInfo whereIn)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> Where(AndOrOption andOrOption, string sql)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> SetColumn(string columnName, string parameterName, bool textParameter, bool function)
        {
            return this;
        }

        public override UpdateContext<Table, Parameter> Append(string sql)
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
    }
}
