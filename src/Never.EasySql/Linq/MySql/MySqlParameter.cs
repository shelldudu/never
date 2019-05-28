using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public class Limit : ISqlarameter
    {
        public Limit(int start)
        {
            this.Start = start;
        }

        public int Start { get; set; }
    }

    public class Limit2 : Limit, ISqlarameter
    {
        public Limit2(int start, int pageSize) : base(start)
        {
            this.PageSize = pageSize;
        }

        public int PageSize { get; set; }
    }
}