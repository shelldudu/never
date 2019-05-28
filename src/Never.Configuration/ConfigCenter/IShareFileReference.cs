using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 共享文件引用
    /// </summary>
    public interface IShareFileReference
    {
        /// <summary>
        /// 文件与共享文件的引用
        /// </summary>
        IConfigurationBuilder Builder { get; }

        /// <summary>
        /// 文件与共享文件的引用
        /// </summary>
        IEnumerable<ShareFileInfo> Reference { get; }
    }
}
