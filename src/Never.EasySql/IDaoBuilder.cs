using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// dao构建者
    /// </summary>
    public interface IDaoBuilder
    {
        /// <summary>
        /// 构建者
        /// </summary>
        Func<IDao> Build { get; }
    }
}
