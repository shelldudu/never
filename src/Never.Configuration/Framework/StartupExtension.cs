#if NET461

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        #region cofig extension

        /// <summary>
        /// 使用Appconfig
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseAppConfig(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseAppConfig"))
                return startup;

            MachineConfig.CurrentConfiguration = MachineConfig.AppConfig;
            startup.ServiceRegister.RegisterInstance(new AppConfigReader(MachineConfig.CurrentConfiguration), typeof(IConfigReader));
            startup.Items["UseAppConfig"] = "t";
            return startup;
        }

        /// <summary>
        /// 使用webconfig
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseWebConfig(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseWebConfig"))
                return startup;

            MachineConfig.CurrentConfiguration = MachineConfig.WebConfig;
            startup.ServiceRegister.RegisterInstance(new AppConfigReader(MachineConfig.CurrentConfiguration), typeof(IConfigReader));
            startup.Items["UseWebConfig"] = "t";
            return startup;
        }

        /// <summary>
        /// 使用Appconfig
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="configReader"></param>
        /// <returns></returns>
        public static ApplicationStartup UseAppConfig(this ApplicationStartup startup, IConfigReader configReader)
        {
            if (startup.Items.ContainsKey("UseAppConfig"))
                return startup;

            startup.ServiceRegister.RegisterInstance(configReader, typeof(IConfigReader));
            startup.Items["UseAppConfig"] = "t";
            return startup;
        }


        #endregion

        #region Configuration读取

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static int IntInAppSettings(this IConfigReader config, string key)
        {
            return config[key].AsInt();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static float FloatInAppSettings(this IConfigReader config, string key)
        {
            return config[key].AsFloat();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static double DoubleInAppSettings(this IConfigReader config, string key)
        {
            return config[key].AsDouble();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static bool BooleanInAppSettings(this IConfigReader config, string key)
        {
            return config[key].AsBool();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static short ShortInAppSettings(this IConfigReader config, string key)
        {
            return config[key].AsShort();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static string StringInAppSettings(this IConfigReader config, string key)
        {
            return config[key];
        }

        /// <summary>
        /// 读取在connectionStrings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static string StringInConnections(this IConfigReader config, string key)
        {
            return config?.Configuration?.ConnectionStrings?.ConnectionStrings[key]?.ConnectionString;
        }

        /// <summary>
        /// 读取在connectionStrings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static System.Configuration.ConnectionStringSettings SettingInConnections(this IConfigReader config, string key)
        {
            return config?.Configuration?.ConnectionStrings?.ConnectionStrings[key];
        }

        /// <summary>
        /// 读取一个Section配置项
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static System.Configuration.ConfigurationSection GetSection(this IConfigReader config, string key)
        {
            return config?.Configuration?.GetSection(key);
        }

        /// <summary>
        /// 读取一个sectionGroup组的值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static System.Configuration.ConfigurationSectionGroup GetSectionGroup(this IConfigReader config, string key)
        {
            return config?.Configuration?.GetSectionGroup(key);
        }

        #endregion Section值读取
    }
}

#endif