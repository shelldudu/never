using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class ConfigFileInfo
    {
        /// <summary>
        /// 文件
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }
    }
}
