using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 请求参数
    /// </summary>
    public interface IRemoteRequest
    {
        #region prop

        /// <summary>
        /// 命令类型
        /// </summary>
        string CommandType { get; }

        #endregion
    }
}
