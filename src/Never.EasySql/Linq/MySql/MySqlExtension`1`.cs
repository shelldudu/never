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
        public static IEnumerable<Result> Select<Table, Parameter, Result>(this IEasySqlQueryable<Table, Parameter> queryable, Expression<IFunc<Table, Predicate<Parameter>, Result>> expression) where Parameter : class
        {
            return Enumerable.Empty<Result>();
        }

        public static Result FirstOrDefault<Table, Parameter, Result>(this IEasySqlQueryable<Table, Parameter> queryable, Expression<IFunc<Table, Predicate<Parameter>, Result>> expression) where Parameter : class
        {
            return default(Result);
        }

        public static IEasySqlQueryable<Table, Parameter> Limit<Table, Parameter>(this IEasySqlQueryable<Table, Parameter> queryable, Expression<IFunc<Table, Predicate<Parameter>, Limit>> expression) where Parameter : class
        {
            return queryable;
        }

        public static IEasySqlQueryable<Table, Parameter> Limit<Table, Parameter>(this IEasySqlQueryable<Table, Parameter> queryable, Expression<IFunc<Table, Predicate<Parameter>, Limit2>> expression) where Parameter : class
        {
            return queryable;
        }

        public static IEasySqlQueryable<Table, Parameter> Where<Table, Parameter>(this IEasySqlQueryable<Table, Parameter> queryable, Expression<IFunc<Table, Predicate<Parameter>, bool>> expression) where Parameter : class
        {
            return queryable;
        }
    }
}