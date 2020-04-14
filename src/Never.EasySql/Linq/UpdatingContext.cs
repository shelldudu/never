using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新语法
    /// </summary>
    public class UpdatingContext<Parameter> : UpdateContext<Parameter>
    {
        private readonly string cacheId;
        private int textLength;
        private int setTimes;
        private string tableName;
        private string asName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public UpdatingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public override int GetResult()
        {
            var sqlTag = new LinqSqlTag(this.cacheId)
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Execute(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        protected override string Format(string text)
        {
            return text;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public override UpdateContext<Parameter> From(string table)
        {
            this.tableName = table;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override UpdateContext<Parameter> AsTable(string table)
        {
            this.asName = table;
            return this;
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override UpdateContext<Parameter> Entrance()
        {
            this.tableName = this.tableName.IsNullOrEmpty() ? this.FindTableName<Parameter>(tableInfo) : this.tableName;
            if (this.asName.IsNullOrEmpty())
            {
                var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.Format(this.tableName), " set \r") };
                this.textLength += label.SqlText.Length;
                this.labels.Add(label);
            }
            else
            {
                var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.Format(this.tableName), " as ", asName, " set \r") };
                this.textLength += label.SqlText.Length;
                this.labels.Add(label);
            }

            return this;
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        protected UpdateContext<Parameter> SetColum<TMember>(string columnName, bool textParameter)
        {
            if (setTimes == 0)
            {
                setTimes++;
                var label = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = string.Concat(this.Format(columnName), " = @", columnName, "\r"),
                };

                label.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = columnName,
                    PositionLength = 1 + columnName.Length + 1,
                    PrefixStart = 1 + columnName.Length + 1 + 3,
                    StartPosition = 1 + columnName.Length + 1 + 3,
                    StopPosition = columnName.Length,
                    TextParameter = textParameter,
                });

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }
            else
            {

                var label = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = string.Concat(",", this.Format(columnName), " = @", columnName, "\r"),
                };

                label.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = columnName,
                    PositionLength = columnName.Length,
                    PrefixStart = 1 + 1 + columnName.Length + 1 + 3,
                    StartPosition = 1 + 1 + columnName.Length + 1 + 3,
                    StopPosition = columnName.Length,
                    TextParameter = textParameter,
                });

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return this;
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            return this.SetColum<TMember>(columnName, false);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColum<TMember>(columnName, true);
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        public override UpdateContext<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.SetColum<TMember>(columnName, false);
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where()
        {
            var label = new TextLabel()
            {
                SqlText = "where 1 = 1 \r",
                TagId = NewId.GenerateNumber(),
            };
            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            string columnName = this.FindColumnName(expression, this.tableInfo, out var member);
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("where ", this.Format(columnName), " = @", columnName, "\r"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = 6 + 1 + columnName.Length + 1,
                PrefixStart = 6 + 1 + columnName.Length + 1 + 3,
                StartPosition = 6 + 1 + columnName.Length + 1 + 3,
                StopPosition = 6 + 1 + columnName.Length + 1,
                TextParameter = false,
            });

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            return this;
        }

        /// <summary>
        /// in
        /// </summary>
        public override UpdateContext<Parameter> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// int
        /// </summary>
        public override UpdateContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            this.Analyze(expression, this.tableInfo, GetTableInfo<Table>(), this.templateParameter, new List<BinaryExp>());
            return this;
        }

        /// <summary>
        /// not in
        /// </summary>
        public override UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? " and " : "  or ", expression),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// not in
        /// </summary>
        public override UpdateContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            this.Analyze(expression, this.tableInfo, GetTableInfo<Table>(), this.templateParameter, new List<BinaryExp>());
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        public override UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// exists
        /// </summary>
        public override UpdateContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            var tableInfo = GetTableInfo<Table>();
            var binary = expression.Body as System.Linq.Expressions.BinaryExpression;
            if (binary != null)
            {
                switch (binary.NodeType)
                {
                    case ExpressionType.Equal:
                        {

                        }
                        break;
                    case ExpressionType.NotEqual:
                        {

                        }
                        break;
                }
                var left = binary.Left;
                var right = binary.Right;

            }

            this.Analyze(expression, this.tableInfo, GetTableInfo<Table>(), this.templateParameter, new List<BinaryExp>());

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

        /// <summary>
        /// not exists
        /// </summary>
        public override UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// not exists
        /// </summary>
        public override UpdateContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            this.Analyze(expression, this.tableInfo, GetTableInfo<Table>(), this.templateParameter, new List<BinaryExp>());
            return this;
        }
    }
}
