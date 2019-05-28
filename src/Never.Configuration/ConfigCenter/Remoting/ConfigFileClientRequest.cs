using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter.Remoting
{
    /// <summary>
    /// 请求服务
    /// </summary>
    public class ConfigFileClientRequest
    {
        /// <summary>
        /// 请求文件名
        /// </summary>
        public string FileName { get; set; }
    }

    /// <summary>
    /// 请求服务
    /// </summary>
    public class ConfigFileClientCallbakRequest: ConfigFileClientRequest
    {
        /// <summary>
        /// 文件编码
        /// </summary>
        public string Encoding { get; set; }
    }
}
