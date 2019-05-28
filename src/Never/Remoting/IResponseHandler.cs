using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 响应处理
    /// </summary>
    public interface IResponseHandler
    {
        /// <summary>
        /// 请求处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponseHandlerResult Excute(IResponseHandlerContext context, IRemoteRequest request);
    }
}
