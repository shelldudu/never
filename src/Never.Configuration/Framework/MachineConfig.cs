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
        /// 打开配置文件，<seealso cref="System.Configuration.ConfigurationManager"/>方法当打开web.config的时候，要求web.config要存在，但是读取内容却是web.config.config文件
        /// </summary>
        public static System.Configuration.Configuration OpenFile(string xmlFileFullName)
        {
            if (xmlFileFullName.IsNullOrWhiteSpace())
                throw new FileNotFoundException("文件路径为空");

            return System.Configuration.ConfigurationManager.OpenExeConfiguration(xmlFileFullName);
        }
    }
}

#endif