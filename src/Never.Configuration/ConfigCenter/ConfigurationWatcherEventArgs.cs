using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 变更事件
    /// </summary>
    public class ConfigurationWatcherEventArgs : EventArgs
    {
        /// <summary>
        /// 配置文件构造者
        /// </summary>
        public IEnumerable<IConfigurationBuilder> Builders { get; set; }
    }
}
