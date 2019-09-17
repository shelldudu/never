using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public struct Update
    {
    }

    public struct Update<T>
    {
        public Update<T> 乐观锁(System.Linq.Expressions.Expression<Func<T, object>> expression, int abc)
        {

        }

        public Update<T> 悲观锁(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {

        }

        public int Change()
        {

        }

        public int Commit()
        {

        }
    }

    public struct Update<T1, T2>
    {

    }
}
