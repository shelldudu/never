using Never.Remoting;
using Never.Remoting.Http;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter.Remoting
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class ConfigFileClient : IWorkService
    {
        private readonly ClientRequestHadler requestHandler = null;
        private Action<ConfigFileClientCallbakRequest, string> saveFile = null;
        private IEnumerable<ConfigFileClientRequest> allFiles = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverEndPoint"></param>
        public ConfigFileClient(EndPoint serverEndPoint)
        {
            this.requestHandler = new ClientRequestHadler(serverEndPoint, new Protocol());
            this.requestHandler.OnMessageReceived += this.RequestHandler_OnMessageReceived;
        }

        private void RequestHandler_OnMessageReceived(object sender, IRemoteResponse response, OnReceivedSocketEventArgs args)
        {
            if (this.allFiles == null || this.saveFile == null)
                return;

            if (response.CommandType.IsEquals(ConfigFileCommand.Pull))
            {
                System.Threading.ThreadPool.QueueUserWorkItem(x =>
                {
                    foreach (var file in this.allFiles)
                    {
                        this.Push(file.FileName);
                    }
                });
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="keepAlive">心跳</param>
        /// <param name="allFiles"></param>
        /// <param name="saveFile"></param>
        public ConfigFileClient Startup(TimeSpan keepAlive, IEnumerable<ConfigFileClientRequest> allFiles, Action<ConfigFileClientCallbakRequest, string> saveFile)
        {
            this.allFiles = allFiles;
            this.saveFile = saveFile;
            this.requestHandler.KeepAlive(keepAlive);
            this.requestHandler.Startup();
            return this;
        }

        /// <summary>
        /// 启动
        /// </summary>
        void IWorkService.Startup()
        {
            if (this.allFiles == null && this.saveFile == null)
                throw new Exception("you must call startup(allfliles,saveFile) method");
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public ConfigFileClient Shutdown()
        {
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
        public IRequestHandler RequestHandler { get { return this.requestHandler; } }

        /// <summary>
        /// 获取数据，会调用Startup里面的回调
        /// </summary>
        /// <param name="name"></param>
        public Task<IRemoteResponse> Push(string name)
        {
            var task = this.requestHandler.Excute(new Request(Encoding.UTF8, ConfigFileCommand.Push)
            {
                Query = new System.Collections.Specialized.NameValueCollection() { { "file", name } }
            });

            return task.Task.ContinueWith(ta =>
            {
                var response = ta.Result as Response;
                var fileName = response.Query["file"];
                if (fileName.IsNotNullOrEmpty() && response.Body != null)
                {
                    var content = response.Encoding.GetString((response.Body as MemoryStream).ToArray());
                    this.saveFile(new ConfigFileClientCallbakRequest() { FileName = fileName, Encoding = response.Query["encoding"] }, content);
                }

                return ta.Result;
            });
        }
    }
}
