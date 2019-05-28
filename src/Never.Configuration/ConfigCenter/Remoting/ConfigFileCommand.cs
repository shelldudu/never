using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter.Remoting
{
    /// <summary>
    /// 文件配置命令
    /// </summary>
    public class ConfigFileCommand
    {
        /// <summary>
        /// 客户端去服务器获取数据的命令
        /// </summary>
        public static string Push = "push";

        /// <summary>
        /// 服务器推送过来的命令
        /// </summary>
        public static string Pull = "pull";
    }
}
