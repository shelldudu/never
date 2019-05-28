using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 配置文件构建
    /// </summary>
    public interface IConfigurationBuilder
    {
        /// <summary>
        /// 名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 文件
        /// </summary>
        ConfigFileInfo File { get; }

        /// <summary>
        /// 文件类型
        /// </summary>
        ConfigFileType FileType { get; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; }

        /// <summary>
        /// 构建
        /// </summary>
        void Rebuild(IEnumerable<ShareConfigurationBuilder> shares);
    }
}
