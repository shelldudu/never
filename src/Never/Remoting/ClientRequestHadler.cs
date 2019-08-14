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
    public class ClientRequestHadler : IRequestHandler, IWorkService, IDisposable
    {
        #region field and ctor

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
            this.Protocol = remoteProtocol;
            if (this.Protocol == null)
                throw new ArgumentNullException("remoteProtocol", "remoteProtocol is null");

            this.all = new ConcurrentDictionary<ulong, CurrentRequestTaskCompletion>();
            this.Socket = new ClientSocket(setting, new SocketBufferProvider(setting), serverEndPoint);
            this.Socket.OnMessageReceived += Client_OnMessageReceived;
            this.Socket.OnConnectionAccepted += (s, e) => { this.OnConnectionAccepted?.Invoke(s, e); };
            this.Socket.OnConnectionClosed += (s, e) => { this.OnConnectionClosed?.Invoke(s, e); };
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
        /// socket
        /// </summary>
        public ClientSocket Socket { get; private set; }

        /// <summary>
        /// protocl
        /// </summary>
        public IRemoteProtocol Protocol { get; private set; }

        /// <summary>
        /// 设置心跳包
        /// </summary>
        /// <param name="keepAlivePeriod">时间间隔</param>
        /// <returns></returns>
        public ClientRequestHadler KeepAlive(TimeSpan keepAlivePeriod)
        {
            if (this.Socket != null)
                this.Socket.KeepAlive(keepAlivePeriod);

            return this;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false)
                return;

            this.Socket.Dispose();
            this.Socket = null;
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
            this.Socket.Push(this.Protocol.FromRequest(current));
            return task;
        }

        private byte[] Client_OnMessageReceived(object sender, OnReceivedSocketEventArgs e)
        {
            var response = this.Protocol.ToResponse(e);
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
            this.Socket.Start(5000);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            this.Socket.Close();
            this.Socket = null;
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
