using Never.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Deployment
{
    /// <summary>
    /// 日志跟踪
    /// </summary>
    public interface IApiRouteLogTracker
    {
        /// <summary>
        /// 日志信息
        /// </summary>
        Func<ILoggerBuilder> LoggerBuilder { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        Task Write(IEnumerable<IApiUriHealthElement> source, IEnumerable<IApiUriHealthElement> match);
    }
}
