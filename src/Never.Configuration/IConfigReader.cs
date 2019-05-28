using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration
{
    /// <summary>
    /// 配置读取
    /// </summary>
    public interface IConfigReader
    {
        /// <summary>
        /// 读取某一值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        string this[string key] { get; }

#if NET461

        /// <summary>
        /// 配置文件
        /// </summary>
        System.Configuration.Configuration Configuration { get; }
#else
        /// <summary>
        /// 配置文件
        /// </summary>
        Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }
#endif
    }
}
