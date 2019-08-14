#if NET461

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Never.Configuration
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class MachineConfig
    {
        /// <summary>
        /// 当前配置文件
        /// </summary>
        public static System.Configuration.Configuration CurrentConfiguration { get; set; }

        /// <summary>
        /// app的配置文件
        /// </summary>
        public static System.Configuration.Configuration AppConfig
        {
            get
            {
                return System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            }
        }

        /// <summary>
        /// web的配置文件
        /// </summary>
        public static System.Configuration.Configuration WebConfig
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            }
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        public static System.Configuration.Configuration OpenFile(FileInfo xmlFile)
        {
            if (xmlFile == null || xmlFile.Exists == false)
                throw new FileNotFoundException("文件不存在");

            return System.Configuration.ConfigurationManager.OpenExeConfiguration(xmlFile.FullName);
        }
    }
}

#endif