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
        public static IEasySqlQueryable<Table1, Table2, Table3, Parameter> LeftJoin<Table1, Table2, Table3, Parameter>(this IEasySqlQueryable<Table1, Table2, Table3, Parameter> queryable, Expression<IFunc<Table1, Table2, Table3, Predicate<Parameter>>> expression) where Parameter : class
        {
            var context = queryable.Context as MySqlEasyContext<Table1, Table2, Table3, Parameter>;
            return queryable;
        }
    }
}