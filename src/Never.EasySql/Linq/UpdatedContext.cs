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
    internal sealed class UpdatedContext<Parameter> : Linq.UpdateContext<Parameter>
    {
        private readonly LinqSqlTag sqlTag;

        public UpdatedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.UpdateContext<Parameter> As(string table)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> StartSetColumn()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> From(string table)
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
            return this.Update(this.sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override Linq.UpdateContext<Parameter> Set<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.templateParameter[this.FindColumnName(expression, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.templateParameter[this.FindColumnName(expression, this.tableInfo, out _)] = value;
            return this;
        }

        public override Linq.UpdateContext<Parameter> Where()
        {
            return this;
        }

        public override Linq.UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }


        protected override string SelectTableNamePointOnSetColunm()
        {
            return string.Empty;
        }

        public override UpdateContext<Parameter> JoinOnUpdate(List<JoinInfo> joins)
        {
            return this;
        }

        public override UpdateContext<Parameter> JoinOnWhereExists(WhereExistsInfo whereExists)
        {
            return this;
        }

        public override UpdateContext<Parameter> JoinOnWhereIn(WhereInInfo whereIn)
        {
            return this;
        }

        public override UpdateContext<Parameter> Where(AndOrOption andOrOption, string sql)
        {
            return this;
        }

        protected override UpdateContext<Parameter> SetColumn(string columnName, string originalColumnName, bool textParameter)
        {
            return this;
        }

        public override UpdateContext<Parameter> Append(string sql)
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
