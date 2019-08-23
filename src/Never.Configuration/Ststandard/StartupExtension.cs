#if !NET461
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
#region config extension

        /// <summary>
        /// 使用appconfig
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseAppConfig(this ApplicationStartup startup, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            if (startup.Items.ContainsKey("UseAppConfig"))
                return startup;

            startup.ServiceRegister.RegisterInstance(new AppConfigReader(configuration), typeof(IConfigReader));
            startup.Items["UseAppConfig"] = "t";
            return startup;
        }

        /// <summary>
        /// 使用appconfig
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

        #endregion config extension

        #region Configuration读取

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static int IntInAppConfig(this IConfigReader config, string key)
        {
            return config[key].AsInt();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static float FloatInAppConfig(this IConfigReader config, string key)
        {
            return config[key].AsFloat();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static double DoubleInAppConfig(this IConfigReader config, string key)
        {
            return config[key].AsDouble();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static bool BooleanInAppConfig(this IConfigReader config, string key)
        {
            return config[key].AsBool();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static short ShortInAppConfig(this IConfigReader config, string key)
        {
            return config[key].AsShort();
        }

        /// <summary>
        /// 读取在appSettings的一个配置值
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="key">配置值</param>
        /// <returns></returns>
        public static string StringInAppConfig(this IConfigReader config, string key)
        {
            return config[key];
        }

        /// <summary>
        /// 根据key读取节点
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Microsoft.Extensions.Configuration.IConfigurationSection GetSection(this IConfigReader config, string key)
        {
            return config?.Configuration?.GetSection(key);
        }

        /// <summary>
        /// 读取子节点
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IEnumerable<Microsoft.Extensions.Configuration.IConfigurationSection> GetChildren(this IConfigReader config)
        {
            return config?.Configuration?.GetChildren();
        }

        /// <summary>
        /// 读取子节点
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Microsoft.Extensions.Primitives.IChangeToken GetReloadToken(this IConfigReader config)
        {
            return config?.Configuration?.GetReloadToken();
        }

#endregion
    }
}
#endif