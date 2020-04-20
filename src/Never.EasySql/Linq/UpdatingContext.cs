﻿using Never.EasySql.Labels;
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
        /// set出现的的次数
        /// </summary>
        protected int setTimes;
        /// <summary>
        /// tablaName
        /// </summary>
        protected string tableName;
        /// <summary>
        /// as别名
        /// </summary>
        protected string asTableName;
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
        protected int formatAppendCount;

        /// <summary>
        /// 等于的前缀
        /// </summary>
        protected string equalAndPrefix;

        #endregion

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
            var sqlTag = new LinqSqlTag(this.cacheId, "update")
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Update(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
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
        /// 别名
        /// </summary>
        public override UpdateContext<Parameter> AsTable(string table)
        {
            this.asTableName = table;
            return this;
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override UpdateContext<Parameter> StartSetColumn()
        {
            this.tableName = this.tableName.IsNullOrEmpty() ? this.FindTableName<Parameter>(tableInfo) : this.tableName;
            int length = this.tableName.Length;
            this.tableName = this.Format(this.tableName);
            this.formatAppendCount = this.tableName.Length - length;

            this.tableNamePoint = string.Concat(this.tableName, ".");
            this.asTableNamePoint = this.asTableName.IsNullOrEmpty() ? string.Empty : string.Concat(this.asTableName, ".");

            var label = this.GetFirstLabelOnEntrance();
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }

        /// <summary>
        /// 获取入口的标签
        /// </summary>
        /// <returns></returns>
        protected virtual TextLabel GetFirstLabelOnEntrance()
        {
            return new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.tableName, "\r", "set") };
        }

        /// <summary>
        /// 更新字段名
        /// </summary>
        protected virtual UpdateContext<Parameter> SetColum<TMember>(string columnName, bool textParameter)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
            };

            var selectTableName = this.SelectTableNamePointOnSetolunm() ?? this.tableNamePoint;

            if (setTimes == 0)
            {
                setTimes++;
                label.SqlText = string.Concat(" ", selectTableName, this.Format(columnName), equalAndPrefix, columnName, "\r");
            }
            else
            {
                label.SqlText = string.Concat(",", selectTableName, this.Format(columnName), equalAndPrefix, columnName, "\r");
            }

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = this.formatAppendCount + columnName.Length,
                PrefixStart = 1 + selectTableName.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                StartPosition = 1 + selectTableName.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                StopPosition = 1 + selectTableName.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
                TextParameter = textParameter,
            });

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;

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
                SqlText = string.Concat("where ", this.asTableNamePoint, this.Format(columnName), this.equalAndPrefix, columnName, "\r"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = columnName,
                PositionLength = this.formatAppendCount + columnName.Length,
                PrefixStart = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                StartPosition = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length,
                StopPosition = 6 + this.asTableNamePoint.Length + this.formatAppendCount + columnName.Length + equalAndPrefix.Length + columnName.Length,
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
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// int
        /// </summary>
        public override UpdateContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and)
        {
            return this.InOrNotIn(option, expression, and, 'i');
        }

        /// <summary>
        /// not in
        /// </summary>
        public override UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// not in
        /// </summary>
        public override UpdateContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> where, Expression<Func<Table, bool>> and)
        {
            return this.InOrNotIn(option, where, and, 'n');
        }

        /// <summary>
        /// in or not in
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="option"></param>
        /// <param name="where"></param>
        /// <param name="and"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected UpdateContext<Parameter> InOrNotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> where, Expression<Func<Table, bool>> and, char flag)
        {
            var collection = new List<BinaryBlock>(10);
            var leftTableInfo = this.tableInfo;
            var rightTableInfo = FindTableInfo<Table>();
            this.Analyze(where, leftTableInfo, rightTableInfo, collection, out var analyzeParameters);
            if (collection.Any() == false)
                return this;

            if (collection.Count > 1)
            {
                collection.RemoveAll(p => p.Join.IsEquals("(") || p.Join.IsEquals(")"));
            }

            if (collection.Any() == false)
                return this;

            if (collection.Count > 1 || collection[0].Join.IsNotEquals(" = ") || collection[0].Left.IsConstant || collection[0].Right.IsConstant)
            {
                throw new Exception("in expression must like this (p,t)=>p.Id == t.Id");
            }

            var leftTableName = this.tableName;
            var rightTableName = this.Format(this.FindTableName<Table>(rightTableInfo));
            var sb = new StringBuilder(collection.Count * 10);
            var leftAs = leftTableName;
            var rightAs = rightTableName;
            if (this.asTableName.IsNotNullOrEmpty())
            {
                leftAs = this.asTableName;
                rightAs = string.Concat(leftAs, leftAs);
                sb.Append(leftAs);
                sb.Append(".");
                sb.Append(collection[0].Join);
                sb.Append(flag == 'n' ? " not in (select " : " in (select ");
                sb.Append(collection[0].Right);
                sb.Append(" from ");
                sb.Append(rightTableName);
                sb.Append(" as ");
                sb.Append(rightAs);
            }
            else
            {
                sb.Append(leftTableName);
                sb.Append(".");
                sb.Append(collection[0].Join);
                sb.Append(flag == 'n' ? " not in (select " : " in (select ");
                sb.Append(collection[0].Right);
                sb.Append(" from ");
                sb.Append(rightTableName);
            }

            if (and != null)
            {
                var temp = new List<BinaryBlock>(10);
                this.Analyze(and, FindTableInfo<Table>(), temp, out _);
                if (collection.Any())
                {
                    sb.Append(" where ");
                    var ppp = analyzeParameters.Select(ta => ta.Placeholder).ToArray();
                    foreach (var c in temp)
                    {
                        sb.Append(c.ToString(ppp));
                    }
                }
            }

            sb.Append(") \r");

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", sb.ToString()),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        public override UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// exists
        /// </summary>
        public override UpdateContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> where, Expression<Func<Table, bool>> and)
        {
            return this.ExistsNotExists(option, where, and, 'e');
        }

        /// <summary>
        /// not exists
        /// </summary>
        public override UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", expression, "\r"),
            };

            this.labels.Add(label);
            return this;
        }

        /// <summary>
        /// not exists
        /// </summary>
        public override UpdateContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and)
        {
            return this.ExistsNotExists(option, expression, and, 'n');
        }

        /// <summary>
        /// exists or not exists
        /// </summary>
        protected UpdateContext<Parameter> ExistsNotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and, char flag)
        {
            var collection = new List<BinaryBlock>(10);
            var leftTableInfo = this.tableInfo;
            var rightTableInfo = FindTableInfo<Table>();
            this.Analyze(expression, leftTableInfo, rightTableInfo, collection, out var analyzeParameters);
            if (collection.Any() == false)
                return this;

            var leftTableName = this.tableName;
            var rightTableName = this.Format(this.FindTableName<Table>(rightTableInfo));
            var sb = new StringBuilder(collection.Count * 10);
            sb.Append(flag == 'n' ? "not exists(select 0 from " : " exists(select 0 from ");
            sb.Append(rightTableName);

            var leftAs = leftTableName;
            var rightAs = rightTableName;
            if (this.asTableName.IsNotNullOrEmpty())
            {
                leftAs = this.asTableName;
                rightAs = string.Concat(leftAs, leftAs);
                sb.Append(" as ");
                sb.Append(rightAs);
            }

            sb.Append(" where ");
            var ppp = analyzeParameters.Select(ta => ta.Placeholder).ToArray();
            for (var i = 0; i < collection.Count; i++)
            {
                if (i == collection.Count - 1)
                {
                    if (and != null)
                    {
                        var temp = new List<BinaryBlock>(10);
                        this.Analyze(and, FindTableInfo<Table>(), temp, out _);
                        if (collection.Any())
                        {
                            sb.Append(" and ");
                            foreach (var c in temp)
                            {
                                sb.Append(c.ToString(ppp));
                            }
                        }
                    }
                    else
                    {
                        sb.Append(collection[i].ToString(ppp));
                    }
                }
                else
                {
                    sb.Append(collection[i].ToString(ppp));
                }
            }

            sb.Append(") \r");

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", sb.ToString()),
            };

            this.labels.Add(label);
            return this;
        }

        protected UpdateContext<Parameter> ExistsNotExists(List<BinaryBlock> onCollection, List<AnalyzeParameter> onAnalyzeParameters, List<BinaryBlock> andCollection, List<AnalyzeParameter> andAnalyzeParameters, char flag)
        {
            var leftTableName = this.tableName;
            var leftAs = leftTableName;
            var sb = new StringBuilder(onCollection.Count * 10);
            sb.Append(flag == 'n' ? "not exists(select 0 from " : " exists(select 0 from ");
            for (int i = 0, j = onCollection.Count; i < j; i++)
            {
                if (onAnalyzeParameters[i].Type == typeof(Parameter))
                    continue;

                sb.Append(FindTableName(onAnalyzeParameters[i].TableInfo, andAnalyzeParameters[i].Type));
                //sb.Append(onCollection[i].ToString())
            }
            var rightTableName = this.Format(this.FindTableName<Table>(rightTableInfo));


            sb.Append(rightTableName);


            var rightAs = rightTableName;
            if (this.asTableName.IsNotNullOrEmpty())
            {
                leftAs = this.asTableName;
                rightAs = string.Concat(leftAs, leftAs);
                sb.Append(" as ");
                sb.Append(rightAs);
            }

            sb.Append(" where ");
            var ppp = onAnalyzeParameters.Select(ta => ta.Placeholder).ToArray();
            for (var i = 0; i < onCollection.Count; i++)
            {
                if (i == onCollection.Count - 1)
                {
                    if (andCollection != null)
                    {
                        if (andCollection.Any())
                        {
                            sb.Append(" and ");
                            var pppp = andAnalyzeParameters.Select(ta => ta.Placeholder).ToArray();
                            foreach (var c in andCollection)
                            {
                                sb.Append(c.ToString(pppp));
                            }
                        }
                    }
                    else
                    {
                        sb.Append(onCollection[i].ToString(ppp));
                    }
                }
                else
                {
                    sb.Append(onCollection[i].ToString(ppp));
                }
            }

            sb.Append(") \r");

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat(option == AndOrOption.and ? "and " : "or ", sb.ToString()),
            };

            this.labels.Add(label);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSetolunm()
        {
            return this.asTableNamePoint;
        }

        public override UpdateContext<Parameter> Exists<Table1, Table2>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, bool>> where, Expression<Func<Table1, Table2, bool>> and)
        {
            throw new NotImplementedException();
        }

        public override UpdateContext<Parameter> NotExists<Table1, Table2>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, bool>> where, Expression<Func<Table1, Table2, bool>> and)
        {
            throw new NotImplementedException();
        }

        public override UpdateContext<Parameter> Exists<Table1, Table2, Table3>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, bool>> where, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            throw new NotImplementedException();
        }

        public override UpdateContext<Parameter> NotExists<Table1, Table2, Table3>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, bool>> where, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            throw new NotImplementedException();
        }

        public override UpdateContext<Parameter> Exists<Table1, Table2, Table3, Table4>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> where, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            throw new NotImplementedException();
        }

        public override UpdateContext<Parameter> NotExists<Table1, Table2, Table3, Table4>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> where, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            throw new NotImplementedException();
        }
    }
}