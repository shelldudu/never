using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 上下文
    /// </summary>
    internal abstract class Context
    {
        /// <summary>
        /// dao
        /// </summary>
        public IDao dao { get; set; }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public abstract SqlTag Build();
    }
}
