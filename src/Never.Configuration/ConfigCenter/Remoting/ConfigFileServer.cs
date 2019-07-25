using Never.Remoting;
using Never.Remoting.Http;
using Never.Sockets.AsyncArgs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Never.Configuration.ConfigCenter.Remoting
{
    /// <summary>
    /// 服务端
    /// </summary>
    public class ConfigFileServer : IResponseHandler, IWorkService
    {
        private readonly ServerRequestHandler requestHandler = null;
        private readonly ConfigurationWatcher configurationWatcher = null;
        private readonly Dictionary<ulong, KeyValuePair<string, Encoding>> sessions = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serverEndPoint"></param>
        /// <param name="configurationWatcher"></param>
        public ConfigFileServer(EndPoint serverEndPoint, ConfigurationWatcher configurationWatcher)
        {
            this.sessions = new Dictionary<ulong, KeyValuePair<string, Encoding>>();
            this.requestHandler = new ServerRequestHandler(serverEndPoint, this, new Protocol());
            this.requestHandler.OnMessageReceived += this.RequestHandler_OnMessageReceived;
            this.requestHandler.OnConnectionClosed += this.RequestHandler_OnConnectionClosed;
            this.configurationWatcher = configurationWatcher;
            this.configurationWatcher.OnAppFileChanged += this.ConfigurationWatcher_OnAppFileChanged;
            this.configurationWatcher.OnAppFileDeleted += this.ConfigurationWatcher_OnAppFileDeleted;
            this.configurationWatcher.OnAppFileRenamed += this.ConfigurationWatcher_OnAppFileRenamed;
            this.configurationWatcher.OnShareFileChanged += this.ConfigurationWatcher_OnShareFileChanged;
            this.configurationWatcher.OnShareFileDeleted += this.ConfigurationWatcher_OnShareFileDeleted;
            this.configurationWatcher.OnShareFileRenamed += this.ConfigurationWatcher_OnShareFileRenamed;
        }

        private void RequestHandler_OnConnectionClosed(object sender, SocketEventArgs e)
        {
            this.sessions.Remove(e.Connection.Id);
        }

        private void RequestHandler_OnMessageReceived(object sender, IRemoteRequest request, OnReceivedSocketEventArgs e2)
        {
            if (this.sessions.ContainsKey(e2.Connection.Id))
                return;

            var reqs = request as Request;
            var fileName = reqs.Query == null ? null : reqs.Query["file"];
            if (fileName == null)
                return;

            this.sessions[e2.Connection.Id] = new KeyValuePair<string, Encoding>(fileName, reqs.Encoding);
        }

        private void ConfigurationWatcher_OnShareFileRenamed(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnShareFileDeleted(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnShareFileChanged(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnAppFileRenamed(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnAppFileDeleted(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnAppFileChanged(object sender, ConfigurationWatcherEventArgs e)
        {
            this.ConfigurationWatcher_OnFileChanged(sender, e);
        }

        private void ConfigurationWatcher_OnFileChanged(object sender, ConfigurationWatcherEventArgs e)
        {
            foreach (var i in e.Builders)
            {
                var all = this.sessions.Where(ta => ta.Value.Key.IsEquals(i.Name));
                if (all.IsNotNullOrEmpty())
                {
                    foreach (var a in all)
                    {
                        var response = new ResponseResult()
                        {
                            Query = new System.Collections.Specialized.NameValueCollection() { { "file", a.Value.Key } },
                        };

                        var request = new Request(a.Value.Value, ConfigFileCommand.Pull)
                        {
                        };

                        var currentRequest = new CurrentRequest()
                        {
                            Id = 0,//Id=0表示服务端主动发送，不用客户端来请求，客户端来请求的话会在协议接口种被编码了
                            Request = request,
                        };

                        this.requestHandler.Send(response, currentRequest, a.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public ConfigFileServer Startup()
        {
            this.requestHandler.Startup();
            return this;
        }

        /// <summary>
        /// 启动
        /// </summary>
        void IWorkService.Startup()
        {
            this.Startup();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public ConfigFileServer Shutdown()
        {
            this.configurationWatcher.Dispose();
            this.requestHandler.Shutdown();
            return this;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        void IWorkService.Shutdown()
        {
            this.Shutdown();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponseHandlerResult IResponseHandler.Excute(IResponseHandlerContext context, IRemoteRequest request)
        {
            var reqs = request as Request;
            var resp = new ResponseResult()
            {
                Query = reqs.Query,
                Headers = reqs.Headers,
                Form = reqs.Form,
            };

            var fileName = reqs.Query == null ? null : reqs.Query["file"];
            if (fileName == null)
                return resp;

            var builder = this.configurationWatcher[fileName];
            if (builder == null)
                return resp;

            resp.Query["encoding"] = builder.File.Encoding.BodyName;
            resp.Body = new MemoryStream(reqs.Encoding.GetBytes(builder.Content));
            return resp;
        }
    }
}