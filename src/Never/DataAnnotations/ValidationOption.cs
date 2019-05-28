using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证选项
    /// </summary>
    public enum ValidationOption : byte
    {
        /// <summary>
        /// 只能中断
        /// </summary>
        @Break = 0,

        /// <summary>
        /// 可以继续
        /// </summary>
        @Continue = 1,
    }
}
