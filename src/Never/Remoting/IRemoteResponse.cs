using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 响应结果
    /// </summary>
    public interface IRemoteResponse
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        string CommandType { get; }
    }
}
