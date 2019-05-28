using Never.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 工作上下文
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// 当前操作者
        /// </summary>
        IWorker Worker { get; }

        /// <summary>
        /// 当前执行时间
        /// </summary>
        DateTime WorkTime { get;  }
    }
}
