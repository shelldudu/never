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
            /// 左边
            /// </summary>
            public string Left;

            /// <summary>
            /// 左边是否常量
            /// </summary>
            public bool LeftIsConstant;

            /// <summary>
            /// 连接符
            /// </summary>
            public string Join;

            /// <summary>
            /// 右边
            /// </summary>
            public string Right;

            /// <summary>
            /// 右边是否常量
            /// </summary>
            public bool RightIsConstant;

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Concat(this.Left ?? "", this.Join ?? "", this.Right ?? "");
            }

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <param name="leftPlaceholder"></param>
            /// <param name="rightPlaceholder"></param>
            /// <returns></returns>
            public string ToString(string leftPlaceholder, string rightPlaceholder)
            {
                return string.Concat(this.Left == null ? "" : (this.LeftIsConstant ? this.Left : string.Concat(leftPlaceholder, ".", Left)),
                    this.Join ?? "",
                    this.Right == null ? "" : (this.RightIsConstant ? this.Right : string.Concat(rightPlaceholder, ".", Right)));
            }
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
        public static TableInfo GetTableInfo<Table>()
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
        protected virtual string FindTableName<Parameter>(TableInfo tableInfo)
        {
            if (tableInfo.TableName != null)
                return tableInfo.TableName.Name;

            return typeof(Parameter).Name;
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="tableInfo"></param>
        /// <param name="templateParameter"></param>
        /// <param name="whereCollection"></param>
        protected virtual void Analyze<Parameter, Table>(Expression<Func<Parameter, Table, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo, IDictionary<string, object> templateParameter, List<BinaryExp> whereCollection)
        {
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return;

            this.Analyze(expression.Parameters[0].Name, typeof(Parameter), expression.Parameters[1].Name, typeof(Table), expression.Body, parameterTableInfo, tableInfo, templateParameter, whereCollection);
        }

        /// <summary>
        /// 分析表达式
        /// </summary>
        /// <param name="leftPlaceholder"></param>
        /// <param name="leftType"></param>
        /// <param name="rightPlaceholder"></param>
        /// <param name="rightType"></param>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="tableInfo"></param>
        /// <param name="templateParameter"></param>
        /// <param name="whereCollection"></param>
        protected void Analyze(string leftPlaceholder, Type leftType, string rightPlaceholder, Type rightType, Expression expression, TableInfo parameterTableInfo, TableInfo tableInfo, IDictionary<string, object> templateParameter, List<BinaryExp> whereCollection)
        {
            var binary = expression as BinaryExpression;
            if (binary == null)
                return;

            //是左边还是右边
            int leftOrRight(Expression expression1)
            {
                if (expression1.Type == leftType || (expression1 is ParameterExpression && ((ParameterExpression)expression1).Name == leftPlaceholder))
                    return 1;

                if (expression1.Type == rightType || (expression1 is ParameterExpression && ((ParameterExpression)expression1).Name == rightPlaceholder))
                    return -1;

                return 0;
            }

            BinaryExp current = null;
            if (binary.Left is BinaryExpression)
            {
                whereCollection.Add(new BinaryExp() { Join = "(" });
                Analyze(leftPlaceholder, leftType, rightPlaceholder, rightType, binary.Left, parameterTableInfo, tableInfo, templateParameter, whereCollection);
                whereCollection.Add(new BinaryExp() { Join = ")" });
            }
            else
            {
                var memberExpress = binary.Left as MemberExpression;
                var leftConfirm = false;
                if (memberExpress != null)
                {
                    /*永远将第一个参数放到左侧，第二个参数放右侧，如果是常量的，则放到右侧*/
                    int isleftOrRight = leftOrRight(memberExpress.Expression);
                    if (isleftOrRight == 1)
                    {
                        current = new BinaryExp()
                        {
                            Left = this.FindColumnName(memberExpress.Member, parameterTableInfo),
                            LeftIsConstant = false,
                        };
                    }
                    else if (isleftOrRight == -1)
                    {
                        current = new BinaryExp()
                        {
                            Right = this.FindColumnName(memberExpress.Member, tableInfo),
                            RightIsConstant = false,
                        };
                    }

                    leftConfirm = true;
                }

                var constantExpress = binary.Left as ConstantExpression;
                if (constantExpress != null)
                {
                    current = new BinaryExp()
                    {
                        Right = constantExpress.Value.ToString(),
                        RightIsConstant = true,
                    };

                    leftConfirm = true;
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
                        whereCollection.Add(new BinaryExp() { Join = "and" });
                    }
                    break;
                case ExpressionType.OrElse:
                    {
                        whereCollection.Add(new BinaryExp() { Join = "or" });
                    }
                    break;
                case ExpressionType.LessThan:
                    {
                        if (current.Left != null)
                            current.Join = " < ";
                        else
                            current.Join = " >= ";
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    {
                        if (current.Left != null)
                            current.Join = " <= ";
                        else
                            current.Join = " > ";
                    }
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    {
                        if (current.Left != null)
                            current.Join = " >= ";
                        else
                            current.Join = " < ";
                    }
                    break;
                case ExpressionType.GreaterThan:
                    {
                        if (current.Left != null)
                            current.Join = " > ";
                        else
                            current.Join = " <= ";
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
                whereCollection.Add(new BinaryExp() { Join = "(" });
                Analyze(leftPlaceholder, leftType, rightPlaceholder, rightType, binary.Right, parameterTableInfo, tableInfo, templateParameter, whereCollection);
                whereCollection.Add(new BinaryExp() { Join = ")" });
            }
            else
            {
                var memberExpress = binary.Right as MemberExpression;
                var rightConfirm = false;
                if (memberExpress != null)
                {
                    if (current.Left != null)
                    {
                        current.Right = this.FindColumnName(memberExpress.Member, tableInfo);
                        current.RightIsConstant = false;
                    }
                    else
                    {
                        current.Left = this.FindColumnName(memberExpress.Member, parameterTableInfo);
                        current.LeftIsConstant = false;
                    }

                    rightConfirm = true;
                }

                var constantExpress = binary.Right as ConstantExpression;
                if (constantExpress != null)
                {
                    if (current.Left != null)
                    {
                        current.Right = constantExpress.Value.ToString();
                        current.RightIsConstant = true;
                    }
                    else
                    {
                        current.Left = constantExpress.Value.ToString();
                        current.LeftIsConstant = true;
                    }

                    rightConfirm = true;
                }

                if (rightConfirm == false)
                {
                    throw new Exception("");
                }
            }

            whereCollection.Add(current);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="expression"></param>
        /// <param name="tableInfo"></param>
        /// <param name="whereCollection"></param>
        protected virtual void Analyze<Parameter>(Expression<Func<Parameter, bool>> expression, TableInfo tableInfo, List<BinaryExp> whereCollection)
        {
            var binary = expression.Body as BinaryExpression;
            if (binary == null)
                return;

            this.Analyze(expression.Parameters[0].Name, typeof(Parameter), expression.Body, tableInfo, whereCollection);
        }

        /// <summary>
        /// 分析表达式
        /// </summary>
        /// <param name="leftPlaceholder"></param>
        /// <param name="leftType"></param>
        /// <param name="expression"></param>
        /// <param name="tableInfo"></param>
        /// <param name="whereCollection"></param>
        protected void Analyze(string leftPlaceholder, Type leftType, Expression expression, TableInfo tableInfo, List<BinaryExp> whereCollection)
        {
            var binary = expression as BinaryExpression;
            if (binary == null)
                return;

            BinaryExp current = null;
            if (binary.Left is BinaryExpression)
            {
                whereCollection.Add(new BinaryExp() { Join = "(" });
                Analyze(leftPlaceholder, leftType, binary.Left, tableInfo, whereCollection);
                whereCollection.Add(new BinaryExp() { Join = ")" });
            }
            else
            {
                var memberExpress = binary.Left as MemberExpression;
                var leftConfirm = false;
                if (memberExpress != null)
                {
                    /*永远将第一个参数放到左侧，第二个参数放右侧，如果是常量的，则放到右侧*/
                    current = new BinaryExp()
                    {
                        Left = this.FindColumnName(memberExpress.Member, tableInfo),
                        LeftIsConstant = false,
                    };

                    leftConfirm = true;
                }

                var constantExpress = binary.Left as ConstantExpression;
                if (constantExpress != null)
                {
                    current = new BinaryExp()
                    {
                        Right = constantExpress.Value.ToString(),
                        RightIsConstant = true,
                    };

                    leftConfirm = true;
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
                        whereCollection.Add(new BinaryExp() { Join = "and" });
                    }
                    break;
                case ExpressionType.OrElse:
                    {
                        whereCollection.Add(new BinaryExp() { Join = "or" });
                    }
                    break;
                case ExpressionType.LessThan:
                    {
                        if (current.Left != null)
                            current.Join = " < ";
                        else
                            current.Join = " >= ";
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    {
                        if (current.Left != null)
                            current.Join = " <= ";
                        else
                            current.Join = " > ";
                    }
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    {
                        if (current.Left != null)
                            current.Join = " >= ";
                        else
                            current.Join = " < ";
                    }
                    break;
                case ExpressionType.GreaterThan:
                    {
                        if (current.Left != null)
                            current.Join = " > ";
                        else
                            current.Join = " <= ";
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
                whereCollection.Add(new BinaryExp() { Join = "(" });
                Analyze(leftPlaceholder, leftType, binary.Right, tableInfo, whereCollection);
                whereCollection.Add(new BinaryExp() { Join = ")" });
            }
            else
            {
                var memberExpress = binary.Right as MemberExpression;
                var rightConfirm = false;
                if (memberExpress != null)
                {
                    current.Left = this.FindColumnName(memberExpress.Member, tableInfo);
                    current.LeftIsConstant = false;

                    rightConfirm = true;
                }

                var constantExpress = binary.Right as ConstantExpression;
                if (constantExpress != null)
                {
                    current.Right = constantExpress.Value.ToString();
                    current.RightIsConstant = true;

                    rightConfirm = true;
                }

                if (rightConfirm == false)
                {
                    throw new Exception("");
                }
            }

            whereCollection.Add(current);
        }

    }
}
