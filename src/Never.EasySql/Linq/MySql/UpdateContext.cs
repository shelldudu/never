using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UpdateContext<Parameter> : Linq.UpdateContext<Parameter>
    {
        private LinqSqlTag sqlTag;
        private int textLength;
        private string cacheId;
        private string tableName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public UpdateContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
            this.tableName = this.FindTableName<Parameter>(tableInfo);
            this.labels.Add(new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.tableName, " set") });
        }

        public override Linq.UpdateContext<Parameter> Exists<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetResult()
        {
            if (this.sqlTag == null)
            {
                this.sqlTag = new LinqSqlTag(this.cacheId)
                {
                    Labels = this.labels.AsEnumerable(),
                    TextLength = this.textLength,
                };

                LinqSqlTagProvider.Set(sqlTag);
            }

            return this.Execute(this.sqlTag, this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string Format(string text)
        {
            return string.Concat("`", text, "`");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public override void AsTable(string table)
        {
            this.tableName = table;
            ((BaseLabel)this.labels[0]).SqlText = string.Concat("update ", this.Format(this.tableName), " as ", table, " set");
        }

        public override Linq.UpdateContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression)
        {
            var tableInfo = EasyDecoratedLinqDao<Parameter>.GetTableInfo<Table>(string.Empty);
            //string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            string columnName = null;
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? " and " : "  or ", "not exists(select 0 from a where a.Id = ", this.Format(columnName), ".Id", columnName),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = 6 + columnName.Length,
                PrefixStart = 6 + columnName.Length + 2 + 3,
                StartPosition = 6 + columnName.Length + 2 + 3,
                StopPosition = 6 + columnName.Length,
                TextParameter = false,
            });

            this.labels.Add(label);
            return this;
        }

        public override Linq.UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? " and " : "  or ", expression),
            };

            this.labels.Add(label);
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColum<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            return this.SetColum(columnName, expression);
        }

        public Linq.UpdateContext<Parameter> SetColum<TMember>(string columnName, Expression<Func<Parameter, TMember>> expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(this.Format(columnName), " = @", columnName),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = columnName.Length,
                PrefixStart = columnName.Length + 2 + 3,
                StartPosition = columnName.Length + 2 + 3,
                StopPosition = columnName.Length,
                TextParameter = false,
            });

            this.labels.Add(label);
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter.Add(columnName, value);
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(this.Format(columnName), " = '", value, "'"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = value.Length,
                PrefixStart = columnName.Length + 2 + 3,
                StartPosition = columnName.Length + 2 + 3,
                StopPosition = columnName.Length,
                TextParameter = true,
            });
            this.labels.Add(label);
            return this;
        }

        public override Linq.UpdateContext<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, object value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter.Add(columnName, value);
            return this.SetColum(columnName, expression);
        }

        public override Linq.UpdateContext<Parameter> Where()
        {
            this.labels.Add(new TextLabel()
            {
                SqlText = " where 1 = 1",
                TagId = NewId.GenerateNumber(),
            });

            return this;
        }

        public override Linq.UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where ", this.Format(columnName), " = @", columnName),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = 6 + columnName.Length,
                PrefixStart = 6 + columnName.Length + 2 + 3,
                StartPosition = 6 + columnName.Length + 2 + 3,
                StopPosition = 6 + columnName.Length,
                TextParameter = false,
            });

            this.labels.Add(label);
            return this;
        }
    }
}
