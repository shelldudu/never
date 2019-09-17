using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{

    public struct Delete<Parameter>
    {
        public Result ToResult<Result>()
        {
            return default(Result);
        }

        public int ToChange()
        {
            return 0;
        }

        public Delete<Parameter> Where()
        {
            return this;
        }
    }

    public struct Delete<T1, T2>
    {

    }
}
