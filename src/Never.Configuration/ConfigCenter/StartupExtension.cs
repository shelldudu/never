using Never.Configuration.ConfigCenter.Remoting;
using Never.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileClient"></param>
        /// <returns></returns>
        public static ApplicationStartup UseConfigClient(this ApplicationStartup startup, EndPoint listenPoint, out ConfigFileClient configFileClient)
        {
            var client = new ConfigFileClient(listenPoint);
            configFileClient = client;
            return startup;
        }


        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="appFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseJsonConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            return UseJsonConfigServer(startup, shareFiles, shareFileEventHandler, appFiles, keyValueFinder, listenPoint, TimeSpan.FromMinutes(1), out configFileServer, out configurationWatcher);
        }

        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="fileChangeTimer">fileChangeTimer</param>
        /// <param name="appFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseJsonConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, TimeSpan fileChangeTimer, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            var shareBuilders = shareFiles == null ? new List<ShareConfigurationBuilder>() : shareFiles.Select(ta => new Never.Configuration.ConfigCenter.ShareConfigurationBuilder(ta)).ToList();
            shareBuilders.UseForEach(builder =>
            {
                if (shareFileEventHandler != null)
                    builder.OnBuilding += shareFileEventHandler;

                builder.Build();
            });

            var jsonBuilders = appFiles == null ? new List<JsonConfigurationBuilder>() : appFiles.Select(ta => new Never.Configuration.ConfigCenter.JsonConfigurationBuilder(shareBuilders, ta, keyValueFinder)).ToList();
            jsonBuilders.UseForEach(builder => { builder.Build(); });

            configurationWatcher = new Never.Configuration.ConfigCenter.ConfigurationWatcher(shareBuilders, jsonBuilders, keyValueFinder, fileChangeTimer)
            {
                ShareFileEventHandler = shareFileEventHandler,
            };
            var server = new Never.Configuration.ConfigCenter.Remoting.ConfigFileServer(listenPoint, configurationWatcher);
            configFileServer = server;
            return startup;
        }

        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="appFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseXmlConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            return UseXmlConfigServer(startup, shareFiles, shareFileEventHandler, appFiles, keyValueFinder, listenPoint, TimeSpan.FromMinutes(1), out configFileServer, out configurationWatcher);
        }

        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="fileChangeTimer">fileChangeTimer</param>
        /// <param name="appFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseXmlConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, TimeSpan fileChangeTimer, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            var shareBuilders = shareFiles == null ? new List<ShareConfigurationBuilder>() : shareFiles.Select(ta => new Never.Configuration.ConfigCenter.ShareConfigurationBuilder(ta)).ToList();
            shareBuilders.UseForEach(builder =>
            {
                if (shareFileEventHandler != null)
                    builder.OnBuilding += shareFileEventHandler;

                builder.Build();
            });

            var jsonBuilders = appFiles == null ? new List<XmlConfigurationBuilder>() : appFiles.Select(ta => new Never.Configuration.ConfigCenter.XmlConfigurationBuilder(shareBuilders, ta, keyValueFinder)).ToList();
            jsonBuilders.UseForEach(builder => { builder.Build(); });

            configurationWatcher = new Never.Configuration.ConfigCenter.ConfigurationWatcher(shareBuilders, jsonBuilders, keyValueFinder, fileChangeTimer)
            {
                ShareFileEventHandler = shareFileEventHandler,
            };
            var server = new Never.Configuration.ConfigCenter.Remoting.ConfigFileServer(listenPoint, configurationWatcher);
            configFileServer = server;
            return startup;
        }

        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="appJsonFiles"></param>
        /// <param name="appXmlFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appJsonFiles, IEnumerable<ConfigFileInfo> appXmlFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            return UseConfigServer(startup, shareFiles, shareFileEventHandler, appJsonFiles, appXmlFiles, keyValueFinder, listenPoint, TimeSpan.FromMinutes(1), out configFileServer, out configurationWatcher);
        }

        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="shareFiles"></param>
        /// <param name="shareFileEventHandler"></param>
        /// <param name="fileChangeTimer">fileChangeTimer</param>
        /// <param name="appJsonFiles"></param>
        /// <param name="appXmlFiles"></param>
        /// <param name="keyValueFinder"></param>
        /// <param name="listenPoint"></param>
        /// <param name="configFileServer"></param>
        /// <param name="configurationWatcher"></param>
        /// <returns></returns>
        public static ApplicationStartup UseConfigServer(this ApplicationStartup startup, IEnumerable<ConfigFileInfo> shareFiles, EventHandler<ShareFileEventArgs> shareFileEventHandler, IEnumerable<ConfigFileInfo> appJsonFiles, IEnumerable<ConfigFileInfo> appXmlFiles, ICustomKeyValueFinder keyValueFinder, EndPoint listenPoint, TimeSpan fileChangeTimer, out ConfigFileServer configFileServer, out ConfigurationWatcher configurationWatcher)
        {
            var shareBuilders = shareFiles == null ? new List<ShareConfigurationBuilder>() : shareFiles.Select(ta => new Never.Configuration.ConfigCenter.ShareConfigurationBuilder(ta)).ToList();
            shareBuilders.UseForEach(builder =>
            {
                if (shareFileEventHandler != null)
                    builder.OnBuilding += shareFileEventHandler;

                builder.Build();
            });

            var jsonBuilders = appJsonFiles == null ? new List<Never.Configuration.ConfigCenter.JsonConfigurationBuilder>() : appJsonFiles.Select(ta => new Never.Configuration.ConfigCenter.JsonConfigurationBuilder(shareBuilders, ta, keyValueFinder)).ToList();
            jsonBuilders.UseForEach(builder => { builder.Build(); });

            var xmlBuilders = appXmlFiles == null ? new List<Never.Configuration.ConfigCenter.XmlConfigurationBuilder>() : appXmlFiles.Select(ta => new Never.Configuration.ConfigCenter.XmlConfigurationBuilder(shareBuilders, ta, keyValueFinder)).ToList();
            xmlBuilders.UseForEach(builder => { builder.Build(); });

            var appBuilders = new List<IShareFileReference>();
            if (jsonBuilders.IsNotNullOrEmpty())
                appBuilders.AddRange(jsonBuilders);

            if (xmlBuilders.IsNotNullOrEmpty())
                appBuilders.AddRange(xmlBuilders);

            configurationWatcher = new Never.Configuration.ConfigCenter.ConfigurationWatcher(shareBuilders, appBuilders, keyValueFinder, fileChangeTimer)
            {
                ShareFileEventHandler = shareFileEventHandler,
            };
            var server = new Never.Configuration.ConfigCenter.Remoting.ConfigFileServer(listenPoint, configurationWatcher);
            configFileServer = server;
            return startup;
        }

    }
}
