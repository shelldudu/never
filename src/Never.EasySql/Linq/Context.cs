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
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected abstract string Format(string text);

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
    }
}
