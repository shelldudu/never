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
    /// 插入语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public class InsertingContext<Parameter, Table> : InsertContext<Parameter, Table>
    {
        #region prop
        /// <summary>
        /// 缓存Id
        /// </summary>
        protected readonly string cacheId;

        /// <summary>
        /// 总字符串长度
        /// </summary>
        protected int textLength;

        /// <summary>
        /// 插入记录次数
        /// </summary>
        protected int insertTimes;

        /// <summary>
        /// tableName.
        /// </summary>
        protected string tableNamePoint;

        /// <summary>
        /// asTable.
        /// </summary>
        protected string asTableNamePoint;

        /// <summary>
        /// format格式化后增加的长度
        /// </summary>
        protected int formatColumnAppendCount;

        /// <summary>
        /// 等于的前缀
        /// </summary>
        protected string equalAndPrefix;

        /// <summary>
        /// 插入字段的模板
        /// </summary>
        protected StringBuilder templateBuilder;

        /// <summary>
        /// 插入字段值的参数
        /// </summary>
        protected List<ILabel> valueLabels;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public InsertingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text)
        {
            return text;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        protected override string FormatColumn(string text)
        {
            return text;
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public override Linq.InsertContext<Parameter, Table> StartEntrance()
        {
            if (this.InsertTable.IsNullOrEmpty())
            {
                this.Into(this.FindTableName(this.tableInfo, typeof(Table)));
            }

            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.InsertTable, ".");
            this.asTableNamePoint = string.Empty;

            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("insert into ", this.InsertTable, "(") };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.templateBuilder = new StringBuilder(300);
            this.valueLabels = new List<ILabel>(10);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <returns></returns>
        public override Result GetResult<Result>()
        {
            this.LoadSqlOnGetResulting();
            var sqlTag = new LinqSqlTag(this.cacheId, "insert")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Insert<Table, Parameter, Result>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);

        }

        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            var sqlTag = new LinqSqlTag(this.cacheId, "insert")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            return this.dao.GetSqlTagFormat<Parameter>(sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetResult()
        {
            this.LoadSqlOnGetResulting();
            var sqlTag = new LinqSqlTag(this.cacheId, "insert")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            this.Insert<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void LoadSqlOnGetResulting()
        {
            if (this.templateBuilder.Length == 0)
                return;

            this.templateBuilder.Append(")values(");
            this.templateBuilder.Insert(0, this.labels[0].SqlText);
            ((BaseLabel)this.labels[0]).SqlText = this.templateBuilder.ToString();
            this.textLength += this.labels[0].SqlText.Length;
            if (this.UseBulk)
            {
                var arrayLabel = new ArrayLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    Open = "(",
                    Close = ")",
                    Split = ",",
                };

                this.templateBuilder.Clear();
                int add = 0 - ((TextLabel)valueLabels.FirstOrDefault()).ParameterPositions.ElementAt(0).ParameterStartIndex;
                foreach (TextLabel label in this.valueLabels)
                {
                    this.templateBuilder.Append(label.SqlText);
                    var parameter = label.ParameterPositions.ElementAt(0);
                    add += parameter.ParameterStartIndex;
                    parameter.ParameterStartIndex = add;
                    parameter.ParameterStopIndex = add;
                    parameter.PrefixStartIndex = add;
                    arrayLabel.Line.Add(parameter);
                }

                arrayLabel.Line = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = this.templateBuilder.ToString(),
                };

                this.labels.Add(arrayLabel);
            }
            else
            {
                this.labels.AddRange(this.valueLabels);
                this.labels.Add(new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = ");",
                });
            }

            this.templateBuilder.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override InsertContext<Parameter, Table> InsertLastInsertId<ReturnType>()
        {
            return this;
        }

        /// <summary>
        /// 插入某一个字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="parameterName"></param>
        /// <param name="textParameter"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public override InsertContext<Parameter, Table> InsertColumn(string columnName, string parameterName, bool textParameter, bool function)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            if (textParameter == false)
            {
                if (insertTimes == 0)
                {
                    templateBuilder.Append(this.FormatColumn(columnName));
                    insertTimes++;
                    label.SqlText = string.Concat(this.dao.SqlExecuter.GetParameterPrefix(), parameterName);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + columnName.Length,
                        PrefixStartIndex = 0,
                        ParameterStartIndex = this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
                else
                {
                    templateBuilder.Append(",");
                    templateBuilder.Append(this.FormatColumn(columnName));
                    label.SqlText = string.Concat(",", this.dao.SqlExecuter.GetParameterPrefix(), parameterName);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + columnName.Length,
                        PrefixStartIndex = 1,
                        ParameterStartIndex = 1 + this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = 1 + this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
            }
            else
            {
                if (insertTimes == 0)
                {
                    var equalAndQuotation = function ? "" : "'";
                    templateBuilder.Append(this.FormatColumn(columnName));
                    insertTimes++;
                    label.SqlText = string.Concat(equalAndQuotation, this.dao.SqlExecuter.GetParameterPrefix(), parameterName, equalAndQuotation);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = columnName.Length + 2,
                        PrefixStartIndex = equalAndQuotation.Length + 0,
                        ParameterStartIndex = equalAndQuotation.Length + 0 + this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = equalAndQuotation.Length + 0 + this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }
                else
                {
                    var equalAndQuotation = function ? "," : ",'";
                    var equalAndQuotationEnd = function ? "" : "'";
                    templateBuilder.Append(",");
                    templateBuilder.Append(this.FormatColumn(columnName));
                    label.SqlText = string.Concat(equalAndQuotation, this.dao.SqlExecuter.GetParameterPrefix(), parameterName, equalAndQuotationEnd);
                    label.Add(new SqlTagParameterPosition()
                    {
                        ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                        Name = columnName,
                        OccupanLength = columnName.Length + 2,
                        PrefixStartIndex = equalAndQuotation.Length,
                        ParameterStartIndex = equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                        ParameterStopIndex = equalAndQuotation.Length + this.dao.SqlExecuter.GetParameterPrefix().Length + parameterName.Length - 1,
                        TextParameter = textParameter,
                    });
                }

            }

            this.textLength += label.SqlText.Length;
            this.valueLabels.Add(label);
            return this;
        }
    }
}
