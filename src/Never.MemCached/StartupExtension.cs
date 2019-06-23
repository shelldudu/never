using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Never.Caching;
using Never.Startups;

namespace Never.Memcached
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动memcached支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="servers">服务列表</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemcached(this ApplicationStartup startup, string[] servers, string key = null)
        {
            return UseMemcached(startup, servers, key, null);
        }

        /// <summary>
        /// 启动memcached支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="servers">服务列表</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="compressProtocol"></param>
        /// <returns></returns>
        public static ApplicationStartup UseMemcached(this ApplicationStartup startup, string[] servers, string key = null, ICompressProtocol compressProtocol = null)
        {
            if (startup.Items.ContainsKey("UseMemcached" + key))
                return startup;

            var mem = MemcachedClient.CreateBinaryCached(servers, compressProtocol ?? new GZipCompressProtocol() { });
            startup.ServiceRegister.RegisterInstance(mem, typeof(ICaching), key);
            startup.Items["UseMemcached" + key] = "t";
            return startup;
        }
    }
}
