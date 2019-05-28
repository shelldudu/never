using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 请求处理
    /// </summary>
    public interface IRequestHandler
    {
        /// <summary>
        /// 请求处理
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TaskCompletionSource<IRemoteResponse> Excute(IRemoteRequest request);
    }
}
