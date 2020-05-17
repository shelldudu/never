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
        public class BinaryExp
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
        public class BinaryBlock
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
            /// <param name="context"></param>
            /// <returns></returns>
            public string ToString(string[] leftPlaceholders, Context context)
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
                        sb.Append(context.FormatColumn(this.Right.Exp));
                    }
                }

                return sb.ToString();
            }
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
        protected int Update<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
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
        protected int Update<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
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
        protected int Delete<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
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
        protected int Delete<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
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
        protected int Insert<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return (int)dao.Insert(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行插入（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Insert<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = (int)dao.Insert(sqlTag, sqlParameter);
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
        protected void InsertMany<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
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
        protected void InsertMany<Parameter>(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
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
        /// 
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
            if (TableInfoProvider.TryUpdateTableInfo(typeof(Table), out var tableInfo))
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
            if (TableInfoProvider.TryUpdateTableInfo(type, out var tableInfo))
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
        protected virtual string FindColumnName(LambdaExpression expression, TableInfo tableInfo, out MemberInfo memberInfo)
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
        protected virtual string FindColumnName(TableInfo tableInfo, MemberInfo memberInfo)
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
                        whereCollection.Add(new BinaryBlock() { Join = "(" });
                        Analyze(binary.Left, analyzeParameters, whereCollection);
                        whereCollection.Add(new BinaryBlock() { Join = binary.NodeType == ExpressionType.AndAlso ? " and " : " or " });
                        Analyze(binary.Right, analyzeParameters, whereCollection);
                        whereCollection.Add(new BinaryBlock() { Join = ")" });
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
                    Left = new BinaryExp()
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

            /*.Left不为空，则说明第一个参数是在左侧*/
            switch (binary.NodeType)
            {
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

            memberExpress = binary.Right as MemberExpression;
            var rightConfirm = false;
            if (memberExpress != null)
            {
                int index = leftOrRight(memberExpress.Expression);
                current.Right = new BinaryExp()
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


            if (current == null)
                throw new Exception("current is null");

            whereCollection.Add(current);
            return true;
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5, Table6>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, Table6, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, TableInfo tableInfo6, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4, Table5>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, Table5, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, TableInfo tableInfo5, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter, Table1, Table2, Table3, Table4>(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, TableInfo tableInfo4, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter, Table1, Table2, Table3>(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, TableInfo tableInfo3, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter, Table1, Table2>(Expression<Func<Parameter, Table1, Table2, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo1, TableInfo tableInfo2, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter, Table>(Expression<Func<Parameter, Table, bool>> expression, TableInfo parameterTableInfo, TableInfo tableInfo, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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
        protected bool Analyze<Parameter>(Expression<Func<Parameter, bool>> expression, TableInfo parameterTableInfo, List<BinaryBlock> whereCollection, out List<AnalyzeParameter> analyzeParameters)
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

        #endregion

        #region join exits in

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

            var whereCollection = new List<BinaryBlock>();
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

                if (this.Analyze(item.On.Body, analyzeParameters, whereCollection) == false)
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

                    if (this.Analyze(item.And.Body, analyzeParameters, whereCollection) == true)
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

            var whereCollection = new List<BinaryBlock>();
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

            if (this.Analyze(whereExists.Where.Body, analyzeParameters, whereCollection) == false)
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
                var joinCollection = new List<BinaryBlock>();
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

                    if (this.Analyze(item.On.Body, joinAnalyzeParameters, joinCollection) == false)
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

                        if (this.Analyze(item.And.Body, joinAnalyzeParameters, joinCollection) == true)
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

                if (this.Analyze(whereExists.And.Body, analyzeParameters, whereCollection) == true)
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

            var whereCollection = new List<BinaryBlock>();
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

            if (this.Analyze(whereIn.Field.Body, analyzeParameters, whereCollection) == false)
                return builder;

            if (whereCollection.Count > 1)
                whereCollection.RemoveAll(p => p.Join.IsEquals("(") || p.Join.IsEquals(")"));


            if (whereCollection.Any() == false)
                return builder;

            if (whereCollection.Count > 1 || whereCollection[0].Join.IsNotEquals(" = ") || whereCollection[0].Left.IsConstant || whereCollection[0].Right.IsConstant)
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

                    var joinCollection = new List<BinaryBlock>();
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

                    if (this.Analyze(item.On.Body, joinAnalyzeParameters, joinCollection) == false)
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

                        if (this.Analyze(item.And.Body, joinAnalyzeParameters, joinCollection) == true)
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

                if (this.Analyze(whereIn.Where.Body, analyzeParameters, whereCollection) == true)
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
