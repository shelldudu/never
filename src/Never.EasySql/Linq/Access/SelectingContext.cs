using Never.EasySql.Labels;
using Never.SqlClient;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.Access
{
    /// <summary>
    /// 查询操作
    /// </summary>
    public sealed class SelectingContext<Parameter, Table> : Linq.SelectingContext<Parameter, Table>
    {
        private List<ILabel> leftLabels;
        private List<ILabel> rightLabels;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public SelectingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(cacheId, dao, tableInfo, sqlParameter)
        {
            if (this.PrimaryKeyColumnName.IsNotNullOrEmpty())
            {
                this.leftLabels = new List<ILabel>();
                this.rightLabels = new List<ILabel>();
            }
            else
            {
                throw new NotSupportedException(string.Format("the table {0} has no primary key", this.FromTable));
            }
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where()
        {
            if (this.onWhereInited == false)
            {
                this.onWhereInited = true;
                this.OnWhereInit();
            }

            if (whereCount == 0)
            {
                whereCount++;
                this.AddFromTable(this.leftLabels, out var leftLength);
                this.textLength += leftLength;
                this.AddFromTable(this.rightLabels, out var rightLength);
                this.textLength += rightLength;

                if (this.selectJoin.IsNotNullOrEmpty())
                {
                    var label = this.LoadJoinLabel(this.FromTable, this.AsTable, selectJoin, this.dao);
                    this.leftLabels.Add(label);
                    this.textLength += label.SqlText.Length;
                    this.rightLabels.Add(label);
                    this.textLength += label.SqlText.Length;
                }

                var label2 = new TextLabel()
                {
                    SqlText = "where 1 = 1 \r",
                    TagId = NewId.GenerateNumber(),
                };
                this.leftLabels.Add(label2);
                this.textLength += label2.SqlText.Length;
                this.rightLabels.Add(label2);
                this.textLength += label2.SqlText.Length;

                var label3 = new NotNullLabel()
                {
                    SqlText = " 1 <= 0",
                    TagId = NewId.GenerateNumber(),
                    Parameter = "3ded96c9-d6a8-4b5c-975e-ac81016d63f3",
                    Then = "and ",
                    Labels = new List<ILabel>()
                    {
                        new TextLabel()
                        {
                            SqlText = " 1 <= 0",
                        }
                    }
                };
                this.rightLabels.Add(label3);
                this.textLength += label3.SqlText.Length;
            }

            return this;
        }

        /// <summary>
        /// wehre
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="andOr"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            if (this.onWhereInited == false)
            {
                this.onWhereInited = true;
                this.OnWhereInit();
            }

            if (whereCount == 0)
            {
                whereCount++;
                this.AddFromTable(this.leftLabels, out var leftLength);
                this.textLength += leftLength;
                this.AddFromTable(this.rightLabels, out var rightLength);
                this.textLength += rightLength;

                if (this.selectJoin.IsNotNullOrEmpty())
                {
                    var label = this.LoadJoinLabel(this.FromTable, this.AsTable, selectJoin, this.dao);
                    this.leftLabels.Add(label);
                    this.textLength += label.SqlText.Length;
                    this.rightLabels.Add(label);
                    this.textLength += label.SqlText.Length;
                }

                var label2 = new TextLabel()
                {
                    SqlText = "where ",
                    TagId = NewId.GenerateNumber(),
                };
                this.leftLabels.Add(label2);
                this.textLength += label2.SqlText.Length;
                this.rightLabels.Add(label2);
                this.textLength += label2.SqlText.Length;

                var templateLabels = new List<ILabel>();
                int s = this.labels.Count;
                if (base.AnalyzeWhereExpress(expression, templateLabels, this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable, this.dao.SqlExecuter.GetParameterPrefix()))
                {
                    this.leftLabels.AddRange(templateLabels);
                    this.rightLabels.AddRange(templateLabels);
                    int e = templateLabels.Count;

                    var labelr = new TextLabel()
                    {
                        SqlText = "\r",
                        TagId = NewId.GenerateNumber(),
                    };
                    this.leftLabels.Add(labelr);
                    this.textLength += labelr.SqlText.Length;
                    this.rightLabels.Add(labelr);
                    this.textLength += labelr.SqlText.Length;
                }

                var label3 = new NotEmptyLabel()
                {
                    SqlText = " 1 <= 0",
                    TagId = NewId.GenerateNumber(),
                    Parameter = "3ded96c9-d6a8-4b5c-975e-ac81016d63f3",
                };
                this.rightLabels.Add(label3);
                this.textLength += label3.SqlText.Length;
            }
            else
            {
                var label2 = new TextLabel()
                {
                    SqlText = andOr == null ? "and " : string.Concat(andOr, " "),
                    TagId = NewId.GenerateNumber(),
                };
                this.leftLabels.Add(label2);
                this.textLength += label2.SqlText.Length;
                this.rightLabels.Add(label2);
                this.textLength += label2.SqlText.Length;

                var templateLabels = new List<ILabel>();
                int s = this.labels.Count;
                if (base.AnalyzeWhereExpress(expression, templateLabels, this.AsTable.IsNullOrEmpty() ? this.FromTable : this.AsTable, this.dao.SqlExecuter.GetParameterPrefix()))
                {
                    this.leftLabels.AddRange(templateLabels);
                    this.rightLabels.AddRange(templateLabels);
                    int e = templateLabels.Count;

                    var label3 = new TextLabel()
                    {
                        SqlText = "\r",
                        TagId = NewId.GenerateNumber(),
                    };
                    this.leftLabels.Add(label3);
                    this.textLength += label3.SqlText.Length;
                    this.rightLabels.Add(label3);
                    this.textLength += label3.SqlText.Length;
                }
            }

            return this;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> SetPage()
        {
            return base.SetPage();
        }

        /// <summary>
        /// 查询字段
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="originalColunmName"></param>
        /// <param name="as"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> SelectColumn(string memberName, string originalColunmName, string @as)
        {
            if (this.isSingle)
                return base.SelectColumn(memberName, originalColunmName, @as);

            if (selectTimes == 0)
            {
                selectTimes++;
                var leftLabel = new TextLabel()
                {
                    SqlText = string.Concat("select top @EndIndex ", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r"),
                    TagId = NewId.GenerateNumber(),
                };
                leftLabel.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = "EndIndex",
                    OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length,
                    PrefixStartIndex = "select top ".Length,
                    ParameterStartIndex = "select top ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                    ParameterStopIndex = "select top ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length - 1,
                    TextParameter = true,
                });

                this.leftLabels.Add(leftLabel);

                var rightLabel = new TextLabel()
                {
                    SqlText = string.Concat("select top @StartIndex ", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r"),
                    TagId = NewId.GenerateNumber(),
                };
                rightLabel.Add(new SqlTagParameterPosition()
                {
                    ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                    Name = "StartIndex",
                    OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length,
                    PrefixStartIndex = "select top ".Length,
                    ParameterStartIndex = "select top ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                    ParameterStopIndex = "select top ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length - 1,
                    TextParameter = true,
                });

                this.rightLabels.Add(rightLabel);
            }
            else
            {
                var leftLabel = new TextLabel()
                {
                    SqlText = string.Concat(",", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r"),
                    TagId = NewId.GenerateNumber(),
                };
                this.leftLabels.Add(leftLabel);
                var rightLabel = new TextLabel()
                {
                    SqlText = string.Concat(",", memberName, @as.IsNullOrEmpty() ? "" : " as ", @as, "\r"),
                    TagId = NewId.GenerateNumber(),
                };

                this.rightLabels.Add(rightLabel);
            }

            return this;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override Table GetResult()
        {
            //加载排序
            this.LoadOrderBy(this.leftLabels, this.orderBies, out var leftLength);
            foreach (var left in this.leftLabels)
            {
                this.labels.Add(left);
                this.textLength += left.SqlText.Length;
            }
            this.textLength += leftLength;

            if (leftLength > 0)
                this.orderBies.Clear();

            return base.GetResult();
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            if (this.whereCount == 0)
            {
                this.Where();
            }

            this.labels.Add(new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = "qwertyuiop.* from (",
            });

            //加载排序
            this.LoadOrderBy(this.leftLabels, this.orderBies, out var leftLength);
            foreach (var left in this.leftLabels)
            {
                this.labels.Add(left);
                this.textLength += left.SqlText.Length;
            }
            this.textLength += leftLength;

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = ") as qwertyuiop left join (",
            };

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;


            //加载排序
            this.LoadOrderBy(this.rightLabels, this.orderBies, out var rightLength);
            foreach (var right in this.rightLabels)
            {
                this.labels.Add(right);
                this.textLength += right.SqlText.Length;
            }
            this.textLength += rightLength;
            var label1 = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Format(") as poiuytrewq on qwertyuiop.{0} = poiuytrewq.{0} where iif(poiuytrewq.{0},'0','1') = '1' ", this.PrimaryKeyColumnName),
            };
            this.labels.Add(label1);
            this.textLength += label1.SqlText.Length;

            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            if (startIndex <= 1)
                this.templateParameter["3ded96c9-d6a8-4b5c-975e-ac81016d63f3"] = "t";

            this.templateParameter["StartIndex"] = ((IPageParameterHandler)dao.SqlExecuter).HandleStartIndex(startIndex);
            this.templateParameter["EndIndex"] = ((IPageParameterHandler)dao.SqlExecuter).HandleEndIndex(endIndex);

            LinqSqlTagProvider.Set(sqlTag);
            return this.SelectMany<Parameter, Table>(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        /// <summary>
        /// 添加查询表
        /// </summary>
        private void AddFromTable(out string fromTableSql)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };
            if (this.AsTable.IsNotNullOrEmpty())
            {
                label.SqlText = string.Concat("from ", this.FromTable, " as ", this.AsTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r");
            }
            else
            {
                label.SqlText = string.Concat("from ", this.FromTable, this.selectJoin.IsNotNullOrEmpty() ? " " : "\r");
            }

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;
            fromTableSql = label.SqlText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatText"></param>
        /// <returns></returns>
        public override SqlTagFormat GetSqlTagFormat(bool formatText = false)
        {
            if (this.isSingle)
                return base.GetSqlTagFormat(formatText);

            this.labels.Insert(0, new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = "select qwertyuiop.* from (",
            });

            //加载排序
            this.LoadOrderBy(this.leftLabels, this.orderBies, out var leftLength);
            foreach (var left in this.leftLabels)
            {
                this.labels.Add(left);
                this.textLength += left.SqlText.Length;
            }
            this.textLength += leftLength;

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = ") as qwertyuiop left join (",
            };

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;


            //加载排序
            this.LoadOrderBy(this.rightLabels, this.orderBies, out var rightLength);
            foreach (var right in this.rightLabels)
            {
                this.labels.Add(right);
                this.textLength += right.SqlText.Length;
            }
            this.textLength += rightLength;

            var label1 = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Format(") as poiuytrewq on qwertyuiop.{0} = poiuytrewq.{0} where iif(poiuytrewq.{0},'0','1') = '1' ", this.PrimaryKeyColumnName),
            };
            this.labels.Add(label1);
            this.textLength += label1.SqlText.Length;

            var sqlTag = new LinqSqlTag(this.cacheId, "select")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            this.templateParameter["StartIndex"] = 2;
            this.templateParameter["EndIndex"] = 1;

            return this.dao.GetSqlTagFormat<Parameter>(sqlTag.Clone(this.templateParameter), this.sqlParameter, formatText);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnWhereInit()
        {
            if (this.selectTimes == 0)
                this.SelectAll();
        }

        /// <summary>
        /// 对表格或字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTableAndColumn(string text)
        {
            return string.Concat("[", text, "]");
        }
    }
}
