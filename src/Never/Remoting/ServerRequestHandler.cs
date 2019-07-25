using Never;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 消息来临
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="request"></param>
    /// <param name="args"></param>
    public delegate void OnServerMessageReceived(object sender, IRemoteRequest request, OnReceivedSocketEventArgs args);

    /// <summary>
    /// 响应处理
    /// </summary>
    public class ServerRequestHandler : IWorkService
    {
        #region field and ctor

        private readonly ServerSocket service = null;

        private readonly IRemoteProtocol remoteProtocol = null;

        /// <summary>
        /// 当消息来临的时候
        /// </summary>
        public event OnServerMessageReceived OnMessageReceived;

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
        /// <param name="listeningEndPoint"></param>
        /// <param name="responseHandler"></param>
        /// <param name="remoteProtocol"></param>
        public ServerRequestHandler(EndPoint listeningEndPoint, IResponseHandler responseHandler, IRemoteProtocol remoteProtocol) : this(listeningEndPoint, responseHandler, new SocketSetting(), remoteProtocol)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="listeningEndPoint"></param>
        /// <param name="setting"></param>
        /// <param name="responseHandler"></param>
        /// <param name="remoteProtocol"></param>
        public ServerRequestHandler(EndPoint listeningEndPoint, IResponseHandler responseHandler, SocketSetting setting, IRemoteProtocol remoteProtocol)
        {
            this.remoteProtocol = remoteProtocol;
            if (this.remoteProtocol == null)
                throw new ArgumentNullException("remoteProtocol", "remoteProtocol is null");

            this.service = new ServerSocket(setting, new SocketBufferProvider(setting), listeningEndPoint);
            this.service.OnMessageReceived += Service_OnMessageReceived;
            this.service.OnConnectionAccepted += (s, e) => { this.OnConnectionAccepted?.Invoke(s, e); };
            this.service.OnConnectionClosed += (s, e) => { this.OnConnectionClosed?.Invoke(s, e); };
            this.ResponseHandler = responseHandler;
        }

        #endregion field and ctor

        #region handle

        /// <summary>
        /// 处理上下文
        /// </summary>
        public struct ResponseHandlerContext : IResponseHandlerContext
        {
            /// <summary>
            /// socket事件
            /// </summary>
            public SocketEventArgs EventArgs { get; set; }
        }

        /// <summary>
        /// 响应处理
        /// </summary>
        public IResponseHandler ResponseHandler { get; }

        /// <summary>
        /// 数据到达后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private byte[] Service_OnMessageReceived(object sender, OnReceivedSocketEventArgs e)
        {
            CurrentRequest current;
            try
            {
                current = this.remoteProtocol.ToRequest(e);
            }
            catch
            {
                return null;
            }

            if (this.ResponseHandler == null)
            {
                try
                {
                    this.OnMessageReceived?.Invoke(sender, current.Request, e);
                }
                finally
                {
                }

                return null;
            }

            var response = this.ResponseHandler.Excute(new ResponseHandlerContext() { EventArgs = e }, current.Request);
            if (response == null)
            {
                try
                {
                    this.OnMessageReceived?.Invoke(sender, current.Request, e);
                }
                finally
                {
                }

                return null;
            }

            try
            {
                this.OnMessageReceived?.Invoke(sender, current.Request, e);
            }
            finally
            {
            }

            try
            {
                return this.remoteProtocol.FromResponse(current, response);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Startup()
        {
            this.service.Start();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            this.service.Close();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="sessiongId"></param>
        [Obsolete("因为Id=0这个条件有歧义，因此该方法会被删除，实际上服务器主动发送，没有请求Id的")]
        public void Send(IResponseHandlerResult response, IRemoteRequest request, ulong sessiongId = 0)
        {
            this.Send(response, new CurrentRequest() { Id = 0, Request = request }, sessiongId);
        }

        /// <summary>
        /// 服务器主动发送消息给特定的sessionid客户端
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="sessiongId"></param>
        public void Send(IResponseHandlerResult response, CurrentRequest request, ulong sessiongId = 0)
        {
            var bytes = this.remoteProtocol.FromResponse(request, response);
            this.service.Push(sessiongId, bytes);
        }

        #endregion handle
    }
}