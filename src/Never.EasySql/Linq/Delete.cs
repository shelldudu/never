using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{

    public abstract class Delete<T>
    {
        public Result ToResult<Result>()
        {
            return default(Result);
        }

        public int ToChange()
        {
            return 0;
        }

        public Delete<T> Where()
        {
            return this;
        }
    }

    public abstract class Delete<T1, T2>
    {

    }
}
