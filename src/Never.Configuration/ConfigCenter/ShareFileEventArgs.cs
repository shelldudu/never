using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 共享文件信息
    /// </summary>
    public class ShareFileEventArgs : EventArgs
    {
        /// <summary>
        /// json的配置文件
        /// </summary>
        public ShareFileInfo JsonShareFile { get; }

        /// <summary>
        /// xml的json的配置文件
        /// </summary>
        public ShareFileInfo XmlShareFile { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonShareFile"></param>
        /// <param name="xmlShareFile"></param>
        public ShareFileEventArgs(ShareFileInfo jsonShareFile, ShareFileInfo xmlShareFile)
        {
            this.JsonShareFile = jsonShareFile;
            this.XmlShareFile = xmlShareFile;
        }
    }
}
