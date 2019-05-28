using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public enum ConfigFileType
    {
        /// <summary>
        /// 没有
        /// </summary>
        No = 0,

        /// <summary>
        /// Json
        /// </summary>
        Json = 1,

        /// <summary>
        /// Xml
        /// </summary>
        Xml = 2,
    }
}
