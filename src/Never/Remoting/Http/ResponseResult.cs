using Never;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting.Http
{
    /// <summary>
    /// 处理结果
    /// </summary>
    public class ResponseResult : EventArgs, IResponseHandlerResult
    {
        #region prop

        /// <summary>
        /// body内容
        /// </summary>
        public MemoryStream Body { get; set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        public NameValueCollection Query { get; set; }

        /// <summary>
        /// form参数
        /// </summary>
        public NameValueCollection Form { get; set; }

        /// <summary>
        /// header参数
        /// </summary>
        public NameValueCollection Headers { get; set; }

        #endregion
    }
}
