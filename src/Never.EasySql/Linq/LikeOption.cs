using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// like类型
    /// </summary>
    public enum LikeOption
    {
        /// <summary>
        /// like
        /// </summary>
        Like = 0,

        /// <summary>
        /// left like
        /// </summary>
        LeftLike = 1,

        /// <summary>
        /// right like
        /// </summary>
        RightLike = 2,
    }
}
