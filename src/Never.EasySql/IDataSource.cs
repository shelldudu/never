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
    public interface IDataSource
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 数据工厂
        /// </summary>
        DbProviderFactory ProviderFactory { get; }
    }
}