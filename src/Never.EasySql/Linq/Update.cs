using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public class Update : DMLContext
    {
    }

    public class Update<T> : Update
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

    public class Update<T1, T2> : Update
    {

    }
}
