#if NET461
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration
{
    /// <summary>
    /// 配置读取
    /// </summary>
    public class FuncConfigReader : IConfigReader
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Func<IConfigReader> @delegate = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegate"></param>
        public FuncConfigReader(Func<IConfigReader> @delegate) { this.@delegate = @delegate; }

        /// <summary>
        /// 读取某一值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string this[string key] => this.@delegate()[key];

        /// <summary>
        /// 配置文件
        /// </summary>
        public System.Configuration.Configuration Configuration => this.@delegate().Configuration;
    }
}
#endif