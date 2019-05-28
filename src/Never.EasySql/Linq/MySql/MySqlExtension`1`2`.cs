using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public static partial class MySqlExtension
    {
        public static IEasySqlQueryable<Table1, Table2, Parameter> LeftJoin<Table1, Table2, Parameter>(this IEasySqlQueryable<Table1, Table2, Parameter> queryable, Expression<IFunc<Table1, Table2, Predicate<Parameter>>> expression) where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Parameter>;
            return queryable;
        }

        public static IEasySqlQueryable<Table1, Table2, Parameter> RightJoin<Table1, Table2, Parameter>(this IEasySqlQueryable<Table1, Table2, Parameter> queryable, Expression<IFunc<Table1, Table2, Predicate<Parameter>>> expression) where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Parameter>;
            return queryable;
        }

        public static IEasySqlQueryable<Table1, Table2, Parameter> InnerJoin<Table1, Table2, Parameter>(this IEasySqlQueryable<Table1, Table2, Parameter> queryable, Expression<IFunc<Table1, Table2, Predicate<Parameter>>> expression) where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Parameter>;
            return queryable;
        }

        public static IEasySqlQueryable<Table1, Table2, Parameter> Open<Table1, Table2, Parameter>(this IEasySqlQueryable<Table1, Table2, Parameter> queryable, Expression<IFunc<Table1, Table2, Predicate<Parameter>>> expression, string pen = "(") where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Parameter>;
            return queryable;
        }

        public static IEasySqlQueryable<Table1, Table2, Parameter> Close<Table1, Table2, Parameter>(this IEasySqlQueryable<Table1, Table2, Parameter> queryable, Expression<IFunc<Table1, Table2, Predicate<Parameter>>> expression, string close = ")") where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Parameter>;
            return queryable;
        }
    }
}