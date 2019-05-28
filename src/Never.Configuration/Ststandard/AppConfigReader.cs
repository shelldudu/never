#if !NET461

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration
{
    /// <summary>
    /// App文件读取
    /// </summary>
    public class AppConfigReader : IConfigReader
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        public AppConfigReader(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// 读取某一值
        /// </summary>
        /// <param name="key">key</param>
        public virtual string this[string key] => this.Configuration[key];

        /// <summary>
        /// 配置文件
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 根据key读取节点
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key)
        {
            return this.Configuration.GetSection(key);
        }

        /// <summary>
        /// 读取子节点
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual IEnumerable<Microsoft.Extensions.Configuration.IConfigurationSection> GetChildren()
        {
            return this.Configuration.GetChildren();
        }

        /// <summary>
        /// 读取子节点
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual Microsoft.Extensions.Primitives.IChangeToken GetReloadToken()
        {
            return this.Configuration.GetReloadToken();
        }
    }
}
#endif
