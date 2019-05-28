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
    /// 响应参数
    /// </summary>
    public class Response : EventArgs, IRemoteResponse
    {
        #region prop

        /// <summary>
        /// 请求编码
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; }

        /// <summary>
        /// body内容
        /// </summary>
        public Stream Body { get; set; }

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

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="encoding"></param>
        public Response(Encoding encoding, string commandType)
        {
            this.Encoding = encoding ?? Encoding.UTF8;
            this.CommandType = commandType ?? string.Empty;
            this.Body = null;
        }
        #endregion
    }
}
