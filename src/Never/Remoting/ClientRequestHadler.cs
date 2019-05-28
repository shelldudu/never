using Never;
using Never.Sockets.AsyncArgs;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Never.Remoting
{
    /// <summary>
    /// 消息来临的通知
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="response"></param>
    /// <param name="args"></param>
    public delegate void OnClientMessageReceived(object sender, IRemoteResponse response, OnReceivedSocketEventArgs args);

    /// <summary>
    /// 请求处理
    /// </summary>
    public class ClientRequestHadler : IRequestHandler, IWorkService
    {
        #region field and ctor

        private readonly ClientSocket client = null;
        private readonly IRemoteProtocol remoteProtocol = null;
        private readonly ConcurrentDictionary<ulong, CurrentRequestTaskCompletion> all = null;
        private static long nextid = Randomizer.Next(2000);

        /// <summary>
        /// 当消息来临的时候
        /// </summary>
        public event OnClientMessageReceived OnMessageReceived;

        /// <summary>
        /// 在连接关闭时刻
        /// </summary>
        public event EventHandler<SocketEventArgs> OnConnectionClosed;


        /// <summary>
        /// 在连接建立时刻
        /// </summary>
        public event EventHandler<SocketEventArgs> OnConnectionAccepted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverEndPoint"></param>
        /// <param name="remoteProtocol"></param>
        public ClientRequestHadler(EndPoint serverEndPoint, IRemoteProtocol remoteProtocol) : this(serverEndPoint, new SocketSetting(), remoteProtocol)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverEndPoint"></param>
        /// <param name="setting"></param>
        /// <param name="remoteProtocol"></param>
        public ClientRequestHadler(EndPoint serverEndPoint, SocketSetting setting, IRemoteProtocol remoteProtocol)
        {
            this.remoteProtocol = remoteProtocol;
            if (this.remoteProtocol == null)
                throw new ArgumentNullException("remoteProtocol", "remoteProtocol is null");

            this.all = new ConcurrentDictionary<ulong, CurrentRequestTaskCompletion>();
            this.client = new ClientSocket(setting, new SocketBufferProvider(setting), serverEndPoint);
            this.client.OnMessageReceived += Client_OnMessageReceived;
            this.client.OnConnectionAccepted += (s, e) => { this.OnConnectionAccepted?.Invoke(s, e); };
            this.client.OnConnectionClosed += (s, e) => { this.OnConnectionClosed?.Invoke(s, e); };
        }

        #endregion

        #region nested

        /// <summary>
        /// 当前请求与响应
        /// </summary>
        public struct CurrentRequestTaskCompletion
        {
            /// <summary>
            /// 当前请求
            /// </summary>
            public CurrentRequest Request { get; set; }

            /// <summary>
            /// 当前响应
            /// </summary>
            public TaskCompletionSource<IRemoteResponse> Response { get; set; }
        }

        #endregion

        #region handle

        /// <summary>
        /// 设置心跳包
        /// </summary>
        /// <param name="keepAlivePeriod">时间间隔</param>
        /// <returns></returns>
        public ClientRequestHadler KeepAlive(TimeSpan keepAlivePeriod)
        {
            if (this.client != null)
                this.client.KeepAlive(keepAlivePeriod);

            return this;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TaskCompletionSource<IRemoteResponse> Excute(IRemoteRequest request)
        {
            var current = new CurrentRequest(request, () => this.NextId);
            var task = new TaskCompletionSource<IRemoteResponse>(TaskCreationOptions.None);
            this.all.TryAdd(current.Id, new CurrentRequestTaskCompletion() { Request = current, Response = task });
            this.client.Push(this.remoteProtocol.FromRequest(current));
            return task;
        }

        private byte[] Client_OnMessageReceived(object sender, OnReceivedSocketEventArgs e)
        {
            var response = this.remoteProtocol.ToResponse(e);
            if (this.all.TryRemove(response.Id, out var current))
            {
                current.Response.TrySetResult(response.Response);
            }

            try
            {
                this.OnMessageReceived?.Invoke(sender, response.Response, e);
            }
            finally
            {

            }

            return null;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Startup()
        {
            this.client.Start(5000);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            this.client.Close();
        }

        /// <summary>
        /// 生成Id
        /// </summary>
        private ulong NextId
        {
            get
            {
                var id = (ulong)System.Threading.Interlocked.Increment(ref nextid);
                System.Threading.Interlocked.CompareExchange(ref nextid, 0, long.MaxValue);
                return id;
            }
        }

        #endregion
    }
}
