using Never.Attributes;
using Never.EasySql.Labels;
using Never.Exceptions;
using Never.Utils;
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
        public class BlockSetting
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
            /// 顺序,从0=参数，1=TableInfo
            /// </summary>
            public int Index;
        }

        /// <summary>
        /// 运算表达式 
        /// </summary>
        public class BlockExpression
        {
            /// <summary>
            /// 左边
            /// </summary>
            public BlockSetting Left;

            /// <summary>
            /// 连接符
            /// </summary>
            public string Method;

            /// <summary>
            /// 右边
            /// </summary>
            public BlockSetting Right;

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public virtual string ToString(string[] leftPlaceholders, Context context)
            {
                return string.Empty;
            }

            /// <summary>
            /// 转成label
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <param name="parameterPrefix"></param>
            /// <param name="parameterIndex"></param>
            /// <returns></returns>
            public virtual ILabel ToLabel(string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
            {
                return null;
            }
        }

        /// <summary>
        /// 二进制运算
        /// </summary>
        public class BinaryBlock : BlockExpression
        {
            /// <summary>
            /// 
            /// </summary>
            public BinaryBlock()
            {
                this.Method = string.Empty;
            }

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public override string ToString(string[] leftPlaceholders, Context context)
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
                        sb.Append(context.FormatColumn(this.Left.Exp));
                    }
                }

                sb.Append(this.Method);
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
                        sb.Append(context.FormatColumn(this.Right.Exp));
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 转成label
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <param name="parameterPrefix"></param>
            /// <param name="parameterIndex"></param>
            /// <returns></returns>
            public override ILabel ToLabel(string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
            {
                var label = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                };

                var sb = new StringBuilder(200);
                if (this.Left != null)
                {
                    if (this.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Left.Exp);
                        sb.Append("'");
                    }
                    else if (this.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(this.Left.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = this.Left.Exp,
                            OccupanLength = parameterPrefix.Length + this.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + this.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(this.Left.Exp));
                    }
                }

                sb.Append(this.Method);

                if (this.Right != null)
                {
                    if (this.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Right.Exp);
                        sb.Append("'");
                    }
                    else if (this.Right.Index == parameterIndex)
                    {
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(this.Right.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = this.Right.Exp,
                            OccupanLength = parameterPrefix.Length + this.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + this.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(this.Right.Exp));
                    }
                }

                label.SqlText = sb.ToString();
                return label;
            }
        }

        /// <summary>
        /// 方法块运算
        /// </summary>
        public class MethodBlock : BlockExpression
        {

            /// <summary>
            /// methods
            /// </summary>
            private static readonly IList<MethodInfo> methods;

            /// <summary>
            /// 是否包含
            /// </summary>
            /// <param name="methodInfo"></param>
            /// <returns></returns>
            public static bool Contains(MethodInfo methodInfo)
            {
                if (methodInfo.IsGenericMethod)
                    return methods.Any(t => t == methodInfo.GetGenericMethodDefinition());

                return methods.Any(t => t == methodInfo);
            }

            static MethodBlock()
            {
                methods = typeof(EasySqlExtension).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(t => t.GetAttribute<SummaryAttribute>() != null).ToList();
            }

            /// <summary>
            /// 
            /// </summary>
            public MethodBlock()
            {
                this.Method = string.Empty;
            }

            /// <summary>
            /// 返回字符串
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public override string ToString(string[] leftPlaceholders, Context context)
            {
                if (this.Method.IsEquals("in", StringComparison.OrdinalIgnoreCase))
                    return new InMethodBlock().ToString(this, leftPlaceholders, context);
                if (this.Method.IsEquals("like", StringComparison.OrdinalIgnoreCase))
                    return new LikeMethodBlock().ToString(this, leftPlaceholders, context);
                if (this.Method.IsEquals("leftlike", StringComparison.OrdinalIgnoreCase))
                    return new LeftLikeMethodBlock().ToString(this, leftPlaceholders, context);
                if (this.Method.IsEquals("rightlike", StringComparison.OrdinalIgnoreCase))
                    return new RightLikeMethodBlock().ToString(this, leftPlaceholders, context);

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
                        sb.Append(context.FormatColumn(this.Left.Exp));
                    }
                }

                sb.Append(this.Method);
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
                        sb.Append(context.FormatColumn(this.Right.Exp));
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 转成label
            /// </summary>
            /// <param name="leftPlaceholders"></param>
            /// <param name="context"></param>
            /// <param name="parameterPrefix"></param>
            /// <param name="parameterIndex"></param>
            /// <returns></returns>
            public override ILabel ToLabel(string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
            {
                if (this.Method.IsEquals("in", StringComparison.OrdinalIgnoreCase))
                    return new InMethodBlock().ToLabel(this, leftPlaceholders, context, parameterPrefix, parameterIndex);
                if (this.Method.IsEquals("like", StringComparison.OrdinalIgnoreCase))
                    return new LikeMethodBlock().ToLabel(this, leftPlaceholders, context, parameterPrefix, parameterIndex);
                if (this.Method.IsEquals("leftlike", StringComparison.OrdinalIgnoreCase))
                    return new LeftLikeMethodBlock().ToLabel(this, leftPlaceholders, context, parameterPrefix, parameterIndex);
                if (this.Method.IsEquals("rightlike", StringComparison.OrdinalIgnoreCase))
                    return new RightLikeMethodBlock().ToLabel(this, leftPlaceholders, context, parameterPrefix, parameterIndex);

                var label = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                };

                var sb = new StringBuilder(200);
                if (this.Left != null)
                {
                    if (this.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Left.Exp);
                        sb.Append("'");
                    }
                    else if (this.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(this.Left.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = this.Left.Exp,
                            OccupanLength = parameterPrefix.Length + this.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + this.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(this.Left.Exp));
                    }
                }

                sb.Append(this.Method);

                if (this.Right != null)
                {
                    if (this.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(this.Right.Exp);
                        sb.Append("'");
                    }
                    else if (this.Right.Index == parameterIndex)
                    {
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(this.Right.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = this.Right.Exp,
                            OccupanLength = parameterPrefix.Length + this.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + this.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[this.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(this.Right.Exp));
                    }
                }

                label.SqlText = sb.ToString();
                return label;
            }

            #region netsted
            private struct InMethodBlock
            {
                public string ToString(MethodBlock method, string[] leftPlaceholders, Context context)
                {
                    var sb = new StringBuilder(30);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    sb.Append(" in (");

                    if (method.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(") ");

                    return sb.ToString();
                }

                public ILabel ToLabel(MethodBlock method, string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
                {
                    var arraylabel = new ArrayLabel()
                    {
                        TagId = NewId.GenerateNumber(),
                        Split = ",",
                        Line = new TextLabel(),
                        SqlText = string.Empty,
                    };

                    var sb = new StringBuilder(200);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else if (method.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(method.Left.Exp);
                        arraylabel.Parameter = method.Left.Exp;
                        arraylabel.Line.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Left.Exp,
                            OccupanLength = parameterPrefix.Length + method.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + method.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }


                    sb.Append(" in (");

                    if (method.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else if (method.Right.Index == parameterIndex)
                    {
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(method.Right.Exp);
                        arraylabel.Parameter = method.Right.Exp;
                        arraylabel.Line.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Right.Exp,
                            OccupanLength = parameterPrefix.Length + method.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + method.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(") ");
                    arraylabel.Line.SqlText = sb.ToString();
                    return arraylabel;
                }
            }

            private struct LikeMethodBlock
            {
                public string ToString(MethodBlock method, string[] leftPlaceholders, Context context)
                {
                    var sb = new StringBuilder(30);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    sb.Append(" like ");
                    if (method.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(" ");
                    return sb.ToString();
                }
                public ILabel ToLabel(MethodBlock method, string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
                {
                    var label = new TextLabel()
                    {
                        TagId = NewId.GenerateNumber(),
                    };

                    var sb = new StringBuilder(200);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else if (method.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(method.Left.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Left.Exp,
                            OccupanLength = parameterPrefix.Length + method.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + method.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    sb.Append(" like ");
                    if (method.Right.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else if (method.Right.Index == parameterIndex)
                    {
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(method.Right.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Right.Exp,
                            OccupanLength = parameterPrefix.Length + method.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + method.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(" ");

                    label.SqlText = sb.ToString();
                    return label;
                }
            }
            private struct LeftLikeMethodBlock
            {
                public string ToString(MethodBlock method, string[] leftPlaceholders, Context context)
                {
                    var sb = new StringBuilder(30);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    if (method.Right.IsConstant)
                    {
                        sb.Append(" like '%");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(" like %");
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(" ");
                    return sb.ToString();
                }
                public ILabel ToLabel(MethodBlock method, string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
                {
                    var label = new TextLabel()
                    {
                        TagId = NewId.GenerateNumber(),
                    };

                    var sb = new StringBuilder(200);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else if (method.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(method.Left.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Left.Exp,
                            OccupanLength = parameterPrefix.Length + method.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + method.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    if (method.Right.IsConstant)
                    {
                        sb.Append(" like '%");
                        sb.Append(method.Right.Exp);
                        sb.Append("'");
                    }
                    else if (method.Right.Index == parameterIndex)
                    {
                        sb.Append(" like %");
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(method.Right.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Right.Exp,
                            OccupanLength = parameterPrefix.Length + method.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + method.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(" like %");
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                    }

                    sb.Append(" ");

                    label.SqlText = sb.ToString();
                    return label;
                }
            }
            private struct RightLikeMethodBlock
            {
                public string ToString(MethodBlock method, string[] leftPlaceholders, Context context)
                {
                    var sb = new StringBuilder(30);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    if (method.Right.IsConstant)
                    {
                        sb.Append(" like '");
                        sb.Append(method.Right.Exp);
                        sb.Append("%'");
                    }
                    else
                    {
                        sb.Append(" like ");
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                        sb.Append("%");
                    }

                    sb.Append(" ");
                    return sb.ToString();
                }
                public ILabel ToLabel(MethodBlock method, string[] leftPlaceholders, Context context, string parameterPrefix, int parameterIndex)
                {
                    var label = new TextLabel()
                    {
                        TagId = NewId.GenerateNumber(),
                    };

                    var sb = new StringBuilder(200);
                    if (method.Left.IsConstant)
                    {
                        sb.Append("'");
                        sb.Append(method.Left.Exp);
                        sb.Append("'");
                    }
                    else if (method.Left.Index == parameterIndex)
                    {
                        sb.Append(parameterPrefix);
                        sb.Append(method.Left.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Left.Exp,
                            OccupanLength = parameterPrefix.Length + method.Left.Exp.Length,
                            PrefixStartIndex = 0,
                            ParameterStartIndex = 0 + 1,
                            ParameterStopIndex = 0 + 1 + method.Left.Exp.Length - 1,
                            TextParameter = false,
                        });
                    }
                    else
                    {
                        sb.Append(leftPlaceholders[method.Left.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Left.Exp));
                    }

                    if (method.Right.IsConstant)
                    {
                        sb.Append(" like '");
                        sb.Append(method.Right.Exp);
                        sb.Append("%'");
                    }
                    else if (method.Right.Index == parameterIndex)
                    {
                        sb.Append(" like ");
                        var start = sb.Length;
                        sb.Append(parameterPrefix);
                        sb.Append(method.Right.Exp);
                        label.Add(new SqlTagParameterPosition()
                        {
                            ActualPrefix = parameterPrefix,
                            SourcePrefix = parameterPrefix,
                            Name = method.Right.Exp,
                            OccupanLength = parameterPrefix.Length + method.Right.Exp.Length,
                            PrefixStartIndex = start + 0,
                            ParameterStartIndex = start + 1,
                            ParameterStopIndex = start + 1 + method.Right.Exp.Length - 1,
                            TextParameter = false,
                        });
                        sb.Append("%");
                    }
                    else
                    {
                        sb.Append(" like ");
                        sb.Append(leftPlaceholders[method.Right.Index]);
                        sb.Append(".");
                        sb.Append(context.FormatColumn(method.Right.Exp));
                        sb.Append("%");
                    }

                    sb.Append(" ");

                    label.SqlText = sb.ToString();
                    return label;
                }
            }
            #endregion
        }

        /// <summary>
        /// 分析参数
        /// </summary>
        public struct AnalyzeParameter
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
        public class JoinInfo
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
            public LambdaExpression On;

            /// <summary>
            /// join的and
            /// </summary>
            public LambdaExpression And;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;
        }

        /// <summary>
        /// 获取一个OrderBy
        /// </summary>
        public struct OrderByInfo
        {
            /// <summary>
            /// asc,desc
            /// </summary>
            public string Flag;

            /// <summary>
            /// orderby
            /// </summary>
            public LambdaExpression OrderBy;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type Type;

            /// <summary>
            /// 占位符
            /// </summary>
            public string Placeholder;
        }

        /// <summary>
        /// exists
        /// </summary>
        public class WhereExistsInfo
        {
            /// <summary>
            /// 是否为not exits
            /// </summary>
            public bool NotExists;

            /// <summary>
            /// 当前是用and还是or进来的
            /// </summary>
            public AndOrOption AndOrOption;

            /// <summary>
            /// 第二张表的别名
            /// </summary>
            public string AsName;

            /// <summary>
            /// and
            /// </summary>
            public LambdaExpression And;

            /// <summary>
            /// where
            /// </summary>
            public LambdaExpression Where;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;

            /// <summary>
            /// join
            /// </summary>
            public List<JoinInfo> Joins;
        }

        /// <summary>
        /// in
        /// </summary>
        public class WhereInInfo
        {
            /// <summary>
            /// 是否为not in
            /// </summary>
            public bool NotIn;

            /// <summary>
            /// 当前是用and还是or进来的
            /// </summary>
            public AndOrOption AndOrOption;

            /// <summary>
            /// 第二张表的别名
            /// </summary>
            public string AsName;

            /// <summary>
            /// where
            /// </summary>
            public LambdaExpression Where;

            /// <summary>
            /// field
            /// </summary>
            public LambdaExpression Field;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;

            /// <summary>
            /// join
            /// </summary>
            public List<JoinInfo> Joins;
        }

        #region execute

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Table Select<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.QueryForObject<Table, Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行查询（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Table Select<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.QueryForObject<Table, Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(Table);
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected IEnumerable<Table> SelectMany<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.QueryForEnumerable<Table, Parameter>(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行查询（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected IEnumerable<Table> SelectMany<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.QueryForEnumerable<Table, Parameter>(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(IEnumerable<Table>);
            }
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Update(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行更新（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.Update(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return -1;
            }
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Delete<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Delete(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行删除（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Delete<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.Delete(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return -1;
            }
        }

        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Result Insert<Table, Parameter, Result>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return (Result)dao.Insert(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行插入（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected Result Insert<Table, Parameter, Result>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = (Result)dao.Insert(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return default(Result);
            }
        }

        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected void Insert<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            dao.Insert(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行插入（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected void Insert<Parameter, Table>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                dao.Insert(sqlTag, sqlParameter);
                dao.CommitTransaction();
            }
            catch
            {
                dao.RollBackTransaction();
                return;
            }
        }

        #endregion

        #region format

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected abstract string FormatTable(string text);

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected abstract string FormatColumn(string text);

        /// <summary>
        /// 清空原来格式化字段后重新格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual string ClearThenFormatColumn(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var split = text.Split('.');
            if (split.Length <= 0)
                return text;

            if (split.Length == 1)
                return this.FormatColumn(text);

            var array = new[] { this.FormatTable(split[0]), ".", this.FormatColumn(split[1]) };
            return string.Join("", split.Length <= 2 ? array : array.Union(split.Skip(2)));

        }

        #endregion

        #region table member

        /// <summary>
        /// 查询table信息
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <returns></returns>
        public static TableInfo FindTableInfo<Table>()
        {
            if (TableInfoCachedProvider.TryUpdateTableInfo(typeof(Table), out var tableInfo))
                return tableInfo;

            throw new KeyNotExistedException("table", "table info not found");
        }

        /// <summary>
        /// 查询table信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableInfo FindTableInfo(Type type)
        {
            if (TableInfoCachedProvider.TryUpdateTableInfo(type, out var tableInfo))
                return tableInfo;

            throw new KeyNotExistedException("table", "table info not found");
        }

        /// <summary>
        /// 对表达式的字段提取其名称
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="expression"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public virtual string FindColumnName(LambdaExpression expression, TableInfo tableInfo, out MemberInfo memberInfo)
        {
            return this.FindColumnName(expression, tableInfo, out memberInfo, out _);
        }

        /// <summary>
        /// 对表达式的字段提取其名称
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="expression"></param>
        /// <param name="memberInfo"></param>
        /// <param name="columnInfo"></param>
        /// <returns></returns>
        public virtual string FindColumnName(LambdaExpression expression, TableInfo tableInfo, out MemberInfo memberInfo, out TableInfo.ColumnInfo columnInfo)
        {
            var body = expression.Body;
            var model = body as ParameterExpression;
            if (model != null)
            {
                memberInfo = null;
                IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member.Name.IsEquals(model.Name));
                if (column.Any())
                {
                    columnInfo = column.FirstOrDefault();
                    if (column.FirstOrDefault().Column != null && column.FirstOrDefault().Column.Alias.IsNotNullOrEmpty())
                    {
                        return column.FirstOrDefault().Column.Alias;
                    }

                    return column.FirstOrDefault().Member.Name;
                }

                columnInfo = default(TableInfo.ColumnInfo);
                return model.Name;
            }

            var member = body as MemberExpression;
            if (member != null)
            {
                memberInfo = member.Member;
                IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member.Name.IsEquals(member.Member.Name));
                if (column.Any())
                {
                    columnInfo = column.FirstOrDefault();
                    if (column.FirstOrDefault().Column != null && column.FirstOrDefault().Column.Alias.IsNotNullOrEmpty())
                    {
                        return column.FirstOrDefault().Column.Alias;
                    }

                    return column.FirstOrDefault().Member.Name;
                }

                columnInfo = default(TableInfo.ColumnInfo);
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
                        columnInfo = column.FirstOrDefault();
                        if (column.FirstOrDefault().Column != null && column.FirstOrDefault().Column.Alias.IsNotNullOrEmpty())
                        {
                            return column.FirstOrDefault().Column.Alias;
                        }

                        return column.FirstOrDefault().Member.Name;
                    }
                    columnInfo = default(TableInfo.ColumnInfo);
                    return memberInfo.Name;
                }
            }

            columnInfo = default(TableInfo.ColumnInfo);
            memberInfo = null;
            return null;
        }

        /// <summary>
        /// 查询字段名字
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        protected virtual string FindColumnName(TableInfo tableInfo, MemberInfo memberInfo)
        {
            IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member == memberInfo);
            if (column.Any())
            {
                if (column.FirstOrDefault().Column != null && column.FirstOrDefault().Column.Alias.IsNotNullOrEmpty())
                {
                    return column.FirstOrDefault().Column.Alias;
                }

                return column.FirstOrDefault().Member.Name;
            }

            return memberInfo.Name;
        }

        /// <summary>
        /// 查询字段名字
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="tableInfo"></param>
        /// <param name="columnInfo"></param>
        /// <returns></returns>
        protected virtual string FindColumnName(TableInfo tableInfo, MemberInfo memberInfo, out TableInfo.ColumnInfo columnInfo)
        {
            IEnumerable<TableInfo.ColumnInfo> column = tableInfo.Columns.Where(ta => ta.Member == memberInfo);
            if (column.Any())
            {
                columnInfo = column.FirstOrDefault();
                if (column.FirstOrDefault().Column != null && column.FirstOrDefault().Column.Alias.IsNotNullOrEmpty())
                {
                    return column.FirstOrDefault().Column.Alias;
                }

                return column.FirstOrDefault().Member.Name;
            }

            columnInfo = default(TableInfo.ColumnInfo);
            return memberInfo.Name;
        }
        /// <summary>
        /// 查询tableName
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        protected string FindTableName<Table>(TableInfo tableInfo)
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
        protected string FindTableName(TableInfo tableInfo, Type type)
        {
            if (tableInfo.TableName != null)
                return tableInfo.TableName.Name;

            return type.Name;
        }

        /// <summary>
        /// 返回Join对应的字符串
        /// </summary>
        /// <param name="joinOption"></param>
        /// <returns></returns>
        protected string FindJoinOptionString(JoinOption joinOption)
        {
            switch (joinOption)
            {
                case JoinOption.Join: return "join";
                case JoinOption.InnerJoin: return "inner join";
                case JoinOption.LeftJoin: return "left join";
                case JoinOption.RightJoin: return "right join";
            }

            return joinOption.ToString();
        }

        #endregion

        #region analyze
        /// <summary>
        /// 分析bool的表达式
        /// </summary>
        /// <param name="analyzeParameters"></param>
        /// <param name="expression"></param>
        /// <param name="whereCollection"></param>
        protected bool AnalyzeBooleanExpression(Expression expression, List<AnalyzeParameter> analyzeParameters, List<BlockExpression> whereCollection)
        {
            var binary = expression as BinaryExpression;
            if (binary != null)
                return this.AnalyzeBooleanBinaryExpression(expression, analyzeParameters, whereCollection);

            var method = expression as System.Linq.Expressions.MethodCallExpression;
            if (method != null)
                return this.AnalyzeBooleanMethodExpression(expression, analyzeParameters, whereCollection);

            return false;
        }
        /// <summary>
        /// 分析bool的表达式
        /// </summary>
        /// <param name="analyzeParameters"></param>
        /// <param name="expression"></param>
        /// <param name="whereCollection"></param>
        protected bool AnalyzeBooleanBinaryExpression(Expression expression, List<AnalyzeParameter> analyzeParameters, List<BlockExpression> whereCollection)
        {
            var binary = expression as BinaryExpression;
            //是第一个还是第二个还是第三个，返回值当作索引
            int leftOrRight(Expression exp)
            {
                for (int i = 0, j = analyzeParameters.Count; i < j; i++)
                {
                    var item = analyzeParameters[i];
                    if (exp.Type == item.Type && (exp is ParameterExpression && ((ParameterExpression)exp).Name == item.Placeholder))
                        return i;
                }

                return 0;
            }

            //匹配TableInfo
            TableInfo matchTable(int index)
            {
                return analyzeParameters[index].TableInfo;
            }

            /**/
            switch (binary.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    {
                        whereCollection.Add(new BinaryBlock() { Method = "(" });
                        AnalyzeBooleanExpression(binary.Left, analyzeParameters, whereCollection);
                        whereCollection.Add(new BinaryBlock() { Method = binary.NodeType == ExpressionType.AndAlso ? " and " : " or " });
                        AnalyzeBooleanExpression(binary.Right, analyzeParameters, whereCollection);
                        whereCollection.Add(new BinaryBlock() { Method = ")" });
                        return true;
                    }
            }

            BinaryBlock current = null;
            var memberExpress = binary.Left as MemberExpression;
            var leftConfirm = false;
            if (memberExpress != null)
            {
                int index = leftOrRight(memberExpress.Expression);
                current = new BinaryBlock()
                {
                    Left = new BlockSetting()
                    {
                        Exp = this.FindColumnName(matchTable(index), memberExpress.Member),
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
                        Left = new BlockSetting()
                        {
                            Exp = constantExpress.Value == null ? null : constantExpress.Value.ToString(),
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

            /*.Left不为空，则说明第一个参数是在左侧*/
            switch (binary.NodeType)
            {
                case ExpressionType.LessThan:
                    {
                        current.Method = " < ";
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    {
                        current.Method = " <= ";
                    }
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    {
                        current.Method = " >= ";
                    }
                    break;
                case ExpressionType.GreaterThan:
                    {
                        current.Method = " > ";
                    }
                    break;
                case ExpressionType.Equal:
                    {
                        current.Method = " = ";
                    }
                    break;
                case ExpressionType.NotEqual:
                    {
                        current.Method = " != ";
                    }
                    break;
            }

            memberExpress = binary.Right as MemberExpression;
            var rightConfirm = false;
            if (memberExpress != null)
            {
                int index = leftOrRight(memberExpress.Expression);
                current.Right = new BlockSetting()
                {
                    Exp = this.FindColumnName(matchTable(index), memberExpress.Member),
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
                    current.Right = new BlockSetting()
                    {
                        Exp = constantExpress.Value == null ? null : constantExpress.Value.ToString(),
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


            if (current == null)
                throw new Exception("current is null");

            whereCollection.Add(current);
            return true;
        }
        /// <summary>
        /// 分析bool的表达式
        /// </summary>
        /// <param name="analyzeParameters"></param>
        /// <param name="expression"></param>
        /// <param name="whereCollection"></param>
        protected bool AnalyzeBooleanMethodExpression(Expression expression, List<AnalyzeParameter> analyzeParameters, List<BlockExpression> whereCollection)
        {
            var method = expression as MethodCallExpression;
            if (method.Arguments.Count != 2)
                throw new Exception(string.Format("parameter typs just has two argements"));

            //是第一个还是第二个还是第三个，返回值当作索引
            int leftOrRight(MemberExpression exp)
            {
                for (int i = 0, j = analyzeParameters.Count; i < j; i++)
                {
                    var item = analyzeParameters[i];
                    if (exp.Expression.Type == item.Type && (exp.Expression is ParameterExpression && ((ParameterExpression)exp.Expression).Name == item.Placeholder))
                        return i;
                }

                return 0;
            }

            //匹配TableInfo
            TableInfo matchTable(int index)
            {
                return analyzeParameters[index].TableInfo;
            }

            MethodBlock blockExpression = new MethodBlock();
            if (MethodBlock.Contains(method.Method))
            {
                blockExpression.Method = method.Method.Name;
                //left
                if (method.Arguments[0].NodeType == ExpressionType.Constant)
                {
                    blockExpression.Left = new BlockSetting()
                    {
                        Exp = ((ConstantExpression)method.Arguments[0].Reduce()).Value.ToString(),
                        IsConstant = true,
                        Index = 0,
                    };
                }
                else
                {
                    int index = leftOrRight((System.Linq.Expressions.MemberExpression)method.Arguments[0]);
                    blockExpression.Left = new BlockSetting()
                    {
                        Exp = this.FindColumnName(matchTable(index), ((System.Linq.Expressions.MemberExpression)method.Arguments[0]).Member),
                        IsConstant = false,
                        Index = index,
                    };
                }

                //right
                if (method.Arguments[1].NodeType == ExpressionType.Constant)
                {
                    blockExpression.Right = new BlockSetting()
                    {
                        Exp = ((ConstantExpression)method.Arguments[1].Reduce()).Value.ToString(),
                        IsConstant = true,
                        Index = 1,
                    };
                }
                else
                {
                    int index = leftOrRight((System.Linq.Expressions.MemberExpression)method.Arguments[1]);
                    blockExpression.Right = new BlockSetting()
                    {
                        Exp = this.FindColumnName(matchTable(index), ((System.Linq.Expressions.MemberExpression)method.Arguments[1]).Member),
                        IsConstant = false,
                        Index = index,
                    };
                }

                whereCollection.Add(blockExpression);
                return true;
            }

            throw new Exception(string.Format("custom method is {0} not support", method.Method.Name));
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, Table6, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, TableInfo tableInfo6, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
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
        protected bool Analyze<Parameter, Table1, Table2, Table3>(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
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
        protected bool Analyze<Parameter, Table1, Table2>(Expression<Func<Parameter, Table1, Table2, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
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
        protected bool Analyze<Parameter, Table>(Expression<Func<Parameter, Table, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
        }

        /// <summary>
        /// 分析语句
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="expression"></param>
        /// <param name="parameterTableInfo"></param>
        /// <param name="analyzeParameters"></param>
        /// <param name="whereCollection"></param>
        protected bool Analyze<Parameter>(Expression<Func<Parameter, bool>> expression, TableInfo parameterTableInfo, List<BlockExpression> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

            return this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection);
        }

        #endregion

        #region join exits in

        /// <summary>
        /// 分析where的表达式
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <typeparam name="Table"></typeparam>
        /// <param name="expression"></param>
        /// <param name="collection"></param>
        /// <param name="tableNameOrTableAliasName"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        protected bool AnalyzeWhereExpress<Parameter, Table>(Expression<Func<Parameter, Table, bool>> expression, List<ILabel> collection, string tableNameOrTableAliasName, string parameterPrefix)
        {
            var whereCollection = new List<BlockExpression>();
            var analyzeParameters = new List<AnalyzeParameter>();
            var types = new[] { typeof(Parameter), typeof(Table) };
            for (var k = 0; k < 2; k++)
            {
                analyzeParameters.Add(new AnalyzeParameter()
                {
                    Placeholder = expression.Parameters[k].Name,
                    TableInfo = FindTableInfo(types[k]),
                    Type = types[k],
                });
            }

            if (this.AnalyzeBooleanExpression(expression.Body, analyzeParameters, whereCollection) == false)
                return false;

            var pp = new[] { expression.Parameters[1].Name, tableNameOrTableAliasName };
            if (collection == null)
                collection = new List<ILabel>(whereCollection.Count());

            foreach (var where in whereCollection)
            {
                //1标识第一个是参数，因为索引从0开始
                collection.Add(where.ToLabel(pp, this, parameterPrefix, 0));
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterTableName"></param>
        /// <param name="parameterAsTableName"></param>
        /// <param name="joins"></param>
        /// <returns></returns>
        protected StringBuilder LoadJoin(string parameterTableName, string parameterAsTableName, List<JoinInfo> joins)
        {
            var builder = new StringBuilder(joins.Count * 20);
            var pp = new[] { "" }.Concat(joins.Select(ta => ta.AsName)).ToArray();
            pp[0] = parameterAsTableName.IsNullOrEmpty() ? parameterTableName : parameterAsTableName;

            var whereCollection = new List<BlockExpression>();
            var analyzeParameters = new List<AnalyzeParameter>();
            for (int i = 0, j = joins.Count; i < j; i++)
            {
                var item = joins[i];
                //join是从第二张表开始的
                if (item.Types.Length != (i + 2))
                    throw new Exception(string.Format("parameter typs not padding,the left length is {0}, and the right length is {1}", joins[i].Types.Length, i + 2));

                whereCollection.Clear();
                analyzeParameters.Clear();
                LambdaExpression lambda = item.On;
                for (var k = 0; k < item.Types.Length; k++)
                {
                    analyzeParameters.Add(new AnalyzeParameter()
                    {
                        Placeholder = lambda.Parameters[k].Name,
                        TableInfo = FindTableInfo(item.Types[k]),
                        Type = item.Types[k],
                    });
                }

                if (this.AnalyzeBooleanExpression(item.On.Body, analyzeParameters, whereCollection) == false)
                    continue;

                builder.Append(this.FindJoinOptionString(item.JoinOption));
                var tableInfo = analyzeParameters.Last().TableInfo;
                builder.Append(" ");
                builder.Append(this.FormatTable(this.FindTableName(tableInfo, item.Types.Last())));
                builder.Append(" as ");
                builder.Append(item.AsName);
                builder.Append(" on ");
                foreach (var where in whereCollection)
                {
                    builder.Append(where.ToString(pp, this));
                }

                if (item.And != null)
                {
                    builder.Append(" ");
                    whereCollection.Clear();
                    analyzeParameters.Clear();
                    lambda = item.And;
                    for (var k = 0; k < item.Types.Length; k++)
                    {
                        analyzeParameters.Add(new AnalyzeParameter()
                        {
                            Placeholder = lambda.Parameters[k].Name,
                            TableInfo = FindTableInfo(item.Types[k]),
                            Type = item.Types[k],
                        });
                    }

                    if (this.AnalyzeBooleanExpression(item.And.Body, analyzeParameters, whereCollection) == true)
                    {
                        builder.Append(" and ");
                        foreach (var where in whereCollection)
                        {
                            builder.Append(where.ToString(pp, this));
                        }
                    }
                }

                builder.Append("\r");
            }

            if (joins.Count == 0)
                builder.Append("\r");

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterTableName"></param>
        /// <param name="parameterAsTableName"></param>
        /// <param name="whereExists"></param>
        /// <returns></returns>
        protected StringBuilder LoadWhereExists(string parameterTableName, string parameterAsTableName, WhereExistsInfo whereExists)
        {
            var builder = new StringBuilder((1 + 1 + whereExists.Joins.Count) * 20);
            var pp = new[] { "", whereExists.AsName }.Concat(whereExists.Joins.Select(ta => this.FormatColumn(ta.AsName))).ToArray();
            pp[0] = parameterAsTableName.IsNullOrEmpty() ? parameterTableName : parameterAsTableName;

            var whereCollection = new List<BlockExpression>();
            var analyzeParameters = new List<AnalyzeParameter>();
            LambdaExpression lambda = whereExists.Where;
            if (whereExists.Types.Length != 2)
                throw new Exception(string.Format("parameter typs not padding,the left length is {0}, and the right length is {1}", whereExists.Types.Length, 2));

            for (var k = 0; k < 2; k++)
            {
                analyzeParameters.Add(new AnalyzeParameter()
                {
                    Placeholder = lambda.Parameters[k].Name,
                    TableInfo = FindTableInfo(whereExists.Types[k]),
                    Type = whereExists.Types[k],
                });
            }

            if (this.AnalyzeBooleanExpression(whereExists.Where.Body, analyzeParameters, whereCollection) == false)
                return builder;

            builder.Append(whereExists.AndOrOption == AndOrOption.and ? "and " : "or ");
            builder.Append(whereExists.NotExists ? "not exists (select 0 from " : "exists (select 0 from ");
            var tableInfo = analyzeParameters[0 + 1].TableInfo;
            builder.Append(this.FormatTable(this.FindTableName(tableInfo, whereExists.Types[0 + 1])));
            builder.Append(" as ");
            builder.Append(whereExists.AsName);
            builder.Append(" ");
            if (whereExists.Joins.Any())
            {
                var joinCollection = new List<BlockExpression>();
                var joinAnalyzeParameters = new List<AnalyzeParameter>();
                bool firstJoin = true;
                for (int i = 0, j = whereExists.Joins.Count; i < j; i++)
                {
                    var item = whereExists.Joins[i];
                    //join是从第三张表开始的，第二张表是用于selet 0 from T2
                    if (item.Types.Length != (i + 3))
                        throw new Exception(string.Format("parameter typs not padding,the left length is {0}, and the right length is {1}", whereExists.Joins[i].Types.Length, i + 3));

                    joinCollection.Clear();
                    joinAnalyzeParameters.Clear();
                    lambda = item.On;
                    for (var k = 0; k < item.Types.Length; k++)
                    {
                        joinAnalyzeParameters.Add(new AnalyzeParameter()
                        {
                            Placeholder = lambda.Parameters[k].Name,
                            TableInfo = FindTableInfo(item.Types[k]),
                            Type = item.Types[k],
                        });
                    }

                    if (this.AnalyzeBooleanExpression(item.On.Body, joinAnalyzeParameters, joinCollection) == false)
                        continue;

                    if (firstJoin)
                    {
                        builder.Append("\r");
                        firstJoin = false;
                    }

                    builder.Append(this.FindJoinOptionString(item.JoinOption));
                    builder.Append(" ");
                    var joinTableInfo = analyzeParameters.Last().TableInfo;
                    builder.Append(this.FormatTable(this.FindTableName(joinTableInfo, item.Types.Last())));
                    builder.Append(" as ");
                    builder.Append(item.AsName);
                    builder.Append(" on ");
                    foreach (var where in joinCollection)
                    {
                        builder.Append(where.ToString(pp, this));
                    }

                    if (item.And != null)
                    {
                        builder.Append(" ");
                        joinCollection.Clear();
                        joinAnalyzeParameters.Clear();
                        lambda = item.And;
                        for (var k = 0; k < item.Types.Length; k++)
                        {
                            joinAnalyzeParameters.Add(new AnalyzeParameter()
                            {
                                Placeholder = lambda.Parameters[k].Name,
                                TableInfo = FindTableInfo(item.Types[k]),
                                Type = item.Types[k],
                            });
                        }

                        if (this.AnalyzeBooleanExpression(item.And.Body, joinAnalyzeParameters, joinCollection) == true)
                        {
                            builder.Append("and ");
                            foreach (var where in whereCollection)
                            {
                                builder.Append(where.ToString(pp, this));
                            }
                        }
                    }

                    builder.Append("\r");
                }
            }
            builder.Append("where ");
            foreach (var where in whereCollection)
            {
                builder.Append(where.ToString(pp, this));
            }

            if (whereExists.And != null)
            {
                builder.Append(" ");
                whereCollection.Clear();
                analyzeParameters.Clear();
                lambda = whereExists.And;
                for (var k = 0; k < whereExists.Types.Length; k++)
                {
                    analyzeParameters.Add(new AnalyzeParameter()
                    {
                        Placeholder = lambda.Parameters[k].Name,
                        TableInfo = FindTableInfo(whereExists.Types[k]),
                        Type = whereExists.Types[k],
                    });
                }

                if (this.AnalyzeBooleanExpression(whereExists.And.Body, analyzeParameters, whereCollection) == true)
                {
                    builder.Append("and ");
                    foreach (var where in whereCollection)
                    {
                        builder.Append(where.ToString(pp, this));
                    }
                }
            }

            builder.Append(")\r");
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterTableName"></param>
        /// <param name="parameterAsTableName"></param>
        /// <param name="whereIn"></param>
        /// <returns></returns>
        protected StringBuilder LoadWhereIn(string parameterTableName, string parameterAsTableName, WhereInInfo whereIn)
        {
            var builder = new StringBuilder((1 + 1 + whereIn.Joins.Count) * 20);
            var pp = new[] { "", whereIn.AsName }.Concat(whereIn.Joins.Select(ta => ta.AsName)).ToArray();
            pp[0] = parameterAsTableName.IsNullOrEmpty() ? parameterTableName : parameterAsTableName;

            var whereCollection = new List<BlockExpression>();
            var analyzeParameters = new List<AnalyzeParameter>();
            LambdaExpression lambda = whereIn.Field;
            if (whereIn.Types.Length != 2)
                throw new Exception(string.Format("parameter typs not padding,the left length is {0}, and the right length is {1}", whereIn.Types.Length, 2));

            for (var k = 0; k < 2; k++)
            {
                analyzeParameters.Add(new AnalyzeParameter()
                {
                    Placeholder = lambda.Parameters[k].Name,
                    TableInfo = FindTableInfo(whereIn.Types[k]),
                    Type = whereIn.Types[k],
                });
            }

            if (this.AnalyzeBooleanExpression(whereIn.Field.Body, analyzeParameters, whereCollection) == false)
                return builder;

            if (whereCollection.Count > 1)
                whereCollection.RemoveAll(p => p.Method.IsEquals("(") || p.Method.IsEquals(")"));


            if (whereCollection.Any() == false)
                return builder;

            if (whereCollection.Count > 1 || whereCollection[0].Method.IsNotEquals(" = ") || whereCollection[0].Left.IsConstant || whereCollection[0].Right.IsConstant)
                throw new Exception("in expression must like this (p,t)=>p.Id == t.Id");

            builder.Append(whereIn.AndOrOption == AndOrOption.and ? "and " : "or ");
            if (parameterAsTableName.IsNullOrEmpty())
            {
                builder.Append(parameterTableName);
                builder.Append(".");
                builder.Append(whereCollection[0].Left.Exp);
            }
            else
            {
                builder.Append(parameterAsTableName);
                builder.Append(".");
                builder.Append(whereCollection[0].Left.Exp);
            }

            builder.Append(whereIn.NotIn ? " not in (select " : " in (select ");
            builder.Append(whereIn.AsName);
            builder.Append(".");
            builder.Append(whereCollection[0].Right.Exp);
            builder.Append(" from ");
            builder.Append(this.FormatTable(FindTableName(analyzeParameters[1].TableInfo, analyzeParameters[1].Type)));
            builder.Append(" as ");
            builder.Append(whereIn.AsName);
            builder.Append(" ");
            if (whereIn.Joins.Any())
            {
                bool firstJoin = true;
                for (int i = 0, j = whereIn.Joins.Count; i < j; i++)
                {
                    var item = whereIn.Joins[i];
                    //in是从第三张表开始的，第二张表是用于selet T2.Id from T2
                    if (item.Types.Length != (i + 3))
                        throw new Exception(string.Format("parameter typs not padding,the left length is {0}, and the right length is {1}", whereIn.Joins[i].Types.Length, i + 3));

                    var joinCollection = new List<BlockExpression>();
                    var joinAnalyzeParameters = new List<AnalyzeParameter>();
                    lambda = item.On;
                    for (var k = 0; k < item.Types.Length; k++)
                    {
                        joinAnalyzeParameters.Add(new AnalyzeParameter()
                        {
                            Placeholder = lambda.Parameters[k].Name,
                            TableInfo = FindTableInfo(item.Types[k]),
                            Type = item.Types[k],
                        });
                    }

                    if (this.AnalyzeBooleanExpression(item.On.Body, joinAnalyzeParameters, joinCollection) == false)
                        continue;

                    if (firstJoin)
                    {
                        builder.Append("\r");
                        firstJoin = false;
                    }
                    builder.Append(this.FindJoinOptionString(item.JoinOption));
                    builder.Append(" ");
                    var joinTableInfo = analyzeParameters.Last().TableInfo;
                    builder.Append(this.FormatTable(this.FindTableName(joinTableInfo, item.Types.Last())));
                    builder.Append(" as ");
                    builder.Append(item.AsName);
                    builder.Append(" on ");
                    foreach (var where in joinCollection)
                    {
                        builder.Append(where.ToString(pp, this));
                    }

                    if (item.And != null)
                    {
                        builder.Append(" ");
                        joinCollection.Clear();
                        joinAnalyzeParameters.Clear();
                        lambda = item.And;
                        for (var k = 0; k < item.Types.Length; k++)
                        {
                            joinAnalyzeParameters.Add(new AnalyzeParameter()
                            {
                                Placeholder = lambda.Parameters[k].Name,
                                TableInfo = FindTableInfo(item.Types[k]),
                                Type = item.Types[k],
                            });
                        }

                        if (this.AnalyzeBooleanExpression(item.And.Body, joinAnalyzeParameters, joinCollection) == true)
                        {
                            builder.Append("and ");
                            foreach (var where in whereCollection)
                            {
                                builder.Append(where.ToString(pp, this));
                            }
                        }
                    }

                    builder.Append("\r");
                }
            }

            builder.Append("where ");
            foreach (var where in whereCollection)
            {
                builder.Append(where.ToString(pp, this));
            }

            if (whereIn.Where != null)
            {
                builder.Append(" ");
                whereCollection.Clear();
                analyzeParameters.Clear();
                lambda = whereIn.Where;
                for (var k = 0; k < whereIn.Types.Length; k++)
                {
                    analyzeParameters.Add(new AnalyzeParameter()
                    {
                        Placeholder = lambda.Parameters[k].Name,
                        TableInfo = FindTableInfo(whereIn.Types[k]),
                        Type = whereIn.Types[k],
                    });
                }

                if (this.AnalyzeBooleanExpression(whereIn.Where.Body, analyzeParameters, whereCollection) == true)
                {
                    builder.Append("and ");
                    foreach (var where in whereCollection)
                    {
                        builder.Append(where.ToString(pp, this));
                    }
                }
            }

            builder.Append(")\r");
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBies"></param>
        /// <returns></returns>
        protected StringBuilder LoadOrderBy(List<OrderByInfo> orderBies)
        {
            var builder = new StringBuilder(orderBies.Count * 20);
            if (orderBies.IsNullOrEmpty())
                return builder;

            builder.Append("order by ");
            for (int i = 0, j = orderBies.Count; i < j; i++)
            {
                var item = orderBies[i];
                var tableInfo = FindTableInfo(item.Type);
                var columnName = this.FindColumnName(item.OrderBy, tableInfo, out var memberInfo);

                if (i > 0)
                    builder.Append(",");
                builder.Append(item.Placeholder);
                builder.Append(".");
                builder.Append(this.FormatColumn(columnName));
                builder.Append(" ");
                builder.Append(item.Flag);
            }

            return builder;
        }
        #endregion
    }
}
