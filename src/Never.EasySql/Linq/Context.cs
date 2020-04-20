using Never.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 上下文
    /// </summary>
    public abstract class Context
    {
        /// <summary>
        /// 二进制运算
        /// </summary>
        protected class BinaryExp
        {
            /// <summary>
            /// 表达式
            /// </summary>
            public string Exp;

            /// <summary>
            /// 是否常量
            /// </summary>
            public bool IsConstant;

            /// <summary>
            /// 顺序,从0开始
            /// </summary>
            public int Index;
        }

        /// <summary>
        /// 二进制运算
        /// </summary>
        protected class BinaryBlock
        {
            /// <summary>
            /// 左边
            /// </summary>
            public BinaryExp Left;

            /// <summary>
            /// 连接符
            /// </summary>
            public string Join;

            /// <summary>
            /// 右边
            /// </summary>
            public BinaryExp Right;

            /// <summary>
            /// 
            /// </summary>
            public BinaryBlock()
            {
                this.Join = string.Empty;
            }

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <returns></returns>
            public string ToString(string[] leftPlaceholders)
            {
                var sb = new StringBuilder(30);
                if (this.Left != null)
                {
                    if (this.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Left.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Left.Index]);
                        sb.Append(".");
                        sb.Append(this.Left.Exp);
                    }
                }

                sb.Append(this.Join);
                if (this.Right != null)
                {
                    if (this.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Right.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Right.Index]);
                        sb.Append(".");
                        sb.Append(this.Right.Exp);
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 分析参数
        /// </summary>
        protected struct AnalyzeParameter
        {
            /// <summary>
            /// 参数类型
            /// </summary>
            public Type Type;

            /// <summary>
            /// 占位符
            /// </summary>
            public string Placeholder;

            /// <summary>
            /// 表信息
            /// </summary>
            public TableInfo TableInfo;
        }

        /// <summary>
        /// jion
        /// </summary>
        protected struct JoinStruct
        {
            /// <summary>
            /// join的类型
            /// </summary>
            public JoinOption JoinOption;

            /// <summary>
            /// join后第二张表的别名
            /// </summary>
            public string AsName;

            /// <summary>
            /// join的on
            /// </summary>
            public Expression On;

            /// <summary>
            /// join的and
            /// </summary>
            public Expression And;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;
        }

        /// <summary>
        /// exists
        /// </summary>
        protected struct ExistsStruct
        {
            /// <summary>
            /// 区分是not还是没有not，比如and not 或者and 
            /// </summary>
            public string Flag;

            /// <summary>
            /// 第二张表的别名
            /// </summary>
            public string AsName;

            /// <summary>
            /// and
            /// </summary>
            public Expression And;

            /// <summary>
            /// where
            /// </summary>
            public Expression Where;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;

            /// <summary>
            /// join
            /// </summary>
            public List<JoinStruct> Joins;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected abstract string Format(string text);

        /// <summary>
        /// 查询table信息
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public static TableInfo FindTableInfo<Table>()
        {
            if (TableInfoProvider.TryUpdateTableInfo(typeof(Table), out var tableInfo))
                return tableInfo;

            throw new KeyNotExistedException("table", "table info not found");
        }

        /// <summary>
        /// 对表达式的字段提取其名称
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="tableInfo"></param>
        /// <param name="expression"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        protected virtual string FindColumnName<Parameter, TMember>(Expression<Func<Parameter, TMember>> expression, TableInfo tableInfo, out MemberInfo memberInfo)
        {
            var body = expression.Body;
            var model = body as ParameterExpression;
            if (model != null)
            {
                memberInfo = null;
                IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member.Name.IsEquals(model.Name));
                if (column.Any())
                {
                    if (column.FirstOrDefault().Column != null)
                    {
                        return column.FirstOrDefault().Column.Alias;
                    }

                    return column.FirstOrDefault().Member.Name;
                }

                return model.Name;
            }

            var member = body as MemberExpression;
            if (member != null)
            {
                memberInfo = member.Member;
                IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member.Name.IsEquals(member.Member.Name));
                if (column.Any())
                {
                    if (column.FirstOrDefault().Column != null)
                    {
                        return column.FirstOrDefault().Column.Alias;
                    }

                    return column.FirstOrDefault().Member.Name;
                }

                return memberInfo.Name;
            }

            var unary = body as UnaryExpression;
            if (unary != null)
            {
                var property = unary.Operand as MemberExpression;
                if (property != null)
                {
                    memberInfo = property.Member;
                    IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member.Name.IsEquals(property.Member.Name));
                    if (column.Any())
                    {
                        if (column.FirstOrDefault().Column != null)
                        {
                            return column.FirstOrDefault().Column.Alias;
                        }

                        return column.FirstOrDefault().Member.Name;
                    }

                    return memberInfo.Name;
                }
            }

            memberInfo = null;
            return null;
        }

        /// <summary>
        /// 查询字段名字
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        protected virtual string FindColumnName(MemberInfo memberInfo, TableInfo tableInfo)
        {
            IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member == memberInfo);
            if (column.Any())
            {
                if (column.FirstOrDefault().Column != null)
                {
                    return column.FirstOrDefault().Column.Alias;
                }

                return column.FirstOrDefault().Member.Name;
            }

            return memberInfo.Name;
        }

        /// <summary>
        /// 查询tableName
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        protected virtual string FindTableName<Table>(TableInfo tableInfo)
        {
            if (tableInfo.TableName != null)
                return tableInfo.TableName.Name;

            return typeof(Table).Name;
        }

        /// <summary>
        /// 查询tableName
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string FindTableName(TableInfo tableInfo, Type type)
        {
            if (tableInfo.TableName != null)
                return tableInfo.TableName.Name;

            return type.Name;
        }

        /// <summary>
        /// 分析表达式
        /// </summary>
        /// <param name="analyzeParameters"></param>
        /// <param name="expression"></param>
        /// <param name="whereCollection"></param>
        protected bool Analyze(Expression expression, List<AnalyzeParameter> analyzeParameters, List<BinaryBlock> whereCollection)
        {
            var binary = expression as BinaryExpression;
            if (binary == null)
                return false;

            //是第一个还是第二个还是第三个，返回值当作索引
            int leftOrRight(Expression exp)
            {
                for (int i = 0, j = analyzeParameters.Count; i < j; i++)
                {
                    var item = analyzeParameters[i];
                    if (exp.Type == item.Type || (exp is ParameterExpression && ((ParameterExpression)exp).Name == item.Placeholder))
                        return i;
                }

                return 0;
            }

            //匹配TableInfo
            TableInfo matchTable(int index)
            {
                return analyzeParameters[index].TableInfo;
            }

            BinaryBlock current = null;
            if (binary.Left is BinaryExpression)
            {
                if (((BinaryExpression)binary.Left).Left is BinaryExpression)
                {
                    whereCollection.Add(new BinaryBlock() { Join = "(" });
                    Analyze(binary.Left, analyzeParameters, whereCollection);
                    whereCollection.Add(new BinaryBlock() { Join = ")" });
                }
                else
                {
                    Analyze(binary.Left, analyzeParameters, whereCollection);
                }
            }
            else
            {
                var memberExpress = binary.Left as MemberExpression;
                var leftConfirm = false;
                if (memberExpress != null)
                {
                    int index = leftOrRight(memberExpress.Expression);
                    current = new BinaryBlock()
                    {
                        Left = new BinaryExp()
                        {
                            Exp = this.FindColumnName(memberExpress.Member, matchTable(index)),
                            IsConstant = false,
                            Index = index,
                        }
                    };

                    leftConfirm = true;
                }

                if (leftConfirm == false)
                {
                    var constantExpress = binary.Left as ConstantExpression;
                    if (constantExpress != null)
                    {
                        current = new BinaryBlock()
                        {
                            Left = new BinaryExp()
                            {
                                Exp = constantExpress.Value.ToString(),
                                IsConstant = true,
                                Index = 0,
                            }
                        };

                        leftConfirm = true;
                    }
                }


                if (leftConfirm == false)
                {
                    throw new Exception("");
                }
            }

            /*.Left不为空，则说明第一个参数是在左侧*/
            switch (binary.NodeType)
            {
                case ExpressionType.AndAlso:
                    {
                        whereCollection.Add(new BinaryBlock() { Join = " and " });
                    }
                    break;
                case ExpressionType.OrElse:
                    {
                        whereCollection.Add(new BinaryBlock() { Join = " or " });
                    }
                    break;
                case ExpressionType.LessThan:
                    {
                        current.Join = " < ";
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    {
                        current.Join = " <= ";
                    }
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    {
                        current.Join = " >= ";
                    }
                    break;
                case ExpressionType.GreaterThan:
                    {
                        current.Join = " > ";
                    }
                    break;
                case ExpressionType.Equal:
                    {
                        current.Join = " = ";
                    }
                    break;
                case ExpressionType.NotEqual:
                    {
                        current.Join = " != ";
                    }
                    break;
            }

            if (binary.Right is BinaryExpression)
            {
                if (((BinaryExpression)binary.Right).Right is BinaryExpression)
                {
                    whereCollection.Add(new BinaryBlock() { Join = "(" });
                    Analyze(binary.Right, analyzeParameters, whereCollection);
                    whereCollection.Add(new BinaryBlock() { Join = ")" });
                }
                else
                {
                    Analyze(binary.Right, analyzeParameters, whereCollection);
                }
            }
            else
            {
                var memberExpress = binary.Right as MemberExpression;
                var rightConfirm = false;
                if (memberExpress != null)
                {
                    int index = leftOrRight(memberExpress.Expression);
                    current.Right = new BinaryExp()
                    {
                        Exp = this.FindColumnName(memberExpress.Member, matchTable(index)),
                        IsConstant = false,
                        Index = index,
                    };

                    rightConfirm = true;
                }

                if (rightConfirm == false)
                {
                    var constantExpress = binary.Right as ConstantExpression;
                    if (constantExpress != null)
                    {
                        current.Right = new BinaryExp()
                        {
                            Exp = constantExpress.Value.ToString(),
                            IsConstant = true,
                            Index = 0,
                        };

                        rightConfirm = true;
                    }

                }


                if (rightConfirm == false)
                {
                    throw new Exception("");
                }
            }

            if (current != null)
            {
                whereCollection.Add(current);

                return true;
            }

            throw new Exception("current is null");
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo1"></param>
        /// <param name="tableInfo2"></param>
        /// <param name="tableInfo3"></param>
        /// <param name="tableInfo4"></param>
        /// <param name="tableInfo5"></param>
        /// <param name="tableInfo6"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, Table6, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, TableInfo tableInfo6, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(7)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table1),
                    TableInfo = tableInfo1
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[2].Name,
                    Type =  typeof(Table2),
                    TableInfo = tableInfo2
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[3].Name,
                    Type =  typeof(Table3),
                    TableInfo = tableInfo3
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[4].Name,
                    Type =  typeof(Table4),
                    TableInfo = tableInfo4
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[5].Name,
                    Type =  typeof(Table5),
                    TableInfo = tableInfo5
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[6].Name,
                    Type =  typeof(Table6),
                    TableInfo = tableInfo6
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <typeparam name="Table5"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo1"></param>
        /// <param name="tableInfo2"></param>
        /// <param name="tableInfo3"></param>
        /// <param name="tableInfo4"></param>
        /// <param name="tableInfo5"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(6)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table1),
                    TableInfo = tableInfo1
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[2].Name,
                    Type =  typeof(Table2),
                    TableInfo = tableInfo2
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[3].Name,
                    Type =  typeof(Table3),
                    TableInfo = tableInfo3
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[4].Name,
                    Type =  typeof(Table4),
                    TableInfo = tableInfo4
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[5].Name,
                    Type =  typeof(Table5),
                    TableInfo = tableInfo5
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo1"></param>
        /// <param name="tableInfo2"></param>
        /// <param name="tableInfo3"></param>
        /// <param name="tableInfo4"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table1, Table2, Table3, Table4>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(5)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table1),
                    TableInfo = tableInfo1
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[2].Name,
                    Type =  typeof(Table2),
                    TableInfo = tableInfo2
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[3].Name,
                    Type =  typeof(Table3),
                    TableInfo = tableInfo3
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[4].Name,
                    Type =  typeof(Table4),
                    TableInfo = tableInfo4
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo1"></param>
        /// <param name="tableInfo2"></param>
        /// <param name="tableInfo3"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table1, Table2, Table3>(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(4)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table1),
                    TableInfo = tableInfo1
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[2].Name,
                    Type =  typeof(Table2),
                    TableInfo = tableInfo2
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[3].Name,
                    Type =  typeof(Table3),
                    TableInfo = tableInfo3
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo1"></param>
        /// <param name="tableInfo2"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table1, Table2>(Expression<Func<Parameter, Table1, Table2, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(3)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table1),
                    TableInfo = tableInfo1
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[2].Name,
                    Type =  typeof(Table2),
                    TableInfo = tableInfo2
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="tableInfo"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter, Table>(Expression<Func<Parameter, Table, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(2)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                },
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[1].Name,
                    Type =  typeof(Table),
                    TableInfo = tableInfo
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="whereCollection"></param>
        protected virtual bool Analyze<Parameter>(Expression<Func<Parameter, bool>> expression, TableInfo parameterTableInfo, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
        {
            analyzeParameters = null;
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return false;

            analyzeParameters = new List<AnalyzeParameter>(2)
            {
                new AnalyzeParameter
                {
                    Placeholder = expression.Parameters[0].Name,
                    Type =  typeof(Parameter),
                    TableInfo = parameterTableInfo
                }
            };

            return this.Analyze(expression.Body, analyzeParameters, whereCollection);
        }
    }
}
