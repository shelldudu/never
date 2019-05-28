using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 数据库相关源
    /// </summary>
    public class DefaultDataSource : IDataSource
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// 数据工厂
        /// </summary>
        public DbProviderFactory ProviderFactory { get; internal set; }
    }
}