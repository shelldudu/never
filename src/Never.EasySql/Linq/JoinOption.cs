using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// join类型
    /// </summary>
    public enum JoinOption
    {
        /// <summary>
        /// join
        /// </summary>
        Join = 0,

        /// <summary>
        /// inner join
        /// </summary>
        InnerJoin = 1,

        /// <summary>
        /// left join
        /// </summary>
        LeftJoin = 2,

        /// <summary>
        /// right join
        /// </summary>
        RightJoin = 3,
    }
}
