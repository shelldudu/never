#if NET461

using System;
using System.Collections.Generic;
using System.IO;
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
        /// 配置信息
        /// </summary>
        public System.Configuration.Configuration Configuration { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        public AppConfigReader(System.Configuration.Configuration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// 读取某一值
        /// </summary>
        /// <param name="key">key</param>
        public virtual string this[string key] => this.StringInAppSettings(key);

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public virtual string StringInAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (this.Configuration == null || this.Configuration.AppSettings == null)
                return string.Empty;

            try
            {
                var temp = this.Configuration.AppSettings.Settings[key];
                return temp == null ? string.Empty : temp.Value;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 读取在connectionStrings的一个配置值
        /// </summary>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public virtual System.Configuration.ConnectionStringSettings SettingInConnections(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (this.Configuration == null || this.Configuration.ConnectionStrings == null)
                return null;

            try
            {
                return this.Configuration.ConnectionStrings.ConnectionStrings[key];
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 读取一个Section配置项
        /// </summary>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public virtual System.Configuration.ConfigurationSection GetSection(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (this.Configuration == null)
                return null;

            try
            {
                return this.Configuration.GetSection(key);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 读取一个sectionGroup组的值
        /// </summary>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public virtual System.Configuration.ConfigurationSectionGroup GetSectionGroup(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (this.Configuration == null)
                return null;

            try
            {
                return this.Configuration.GetSectionGroup(key);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
#endif
