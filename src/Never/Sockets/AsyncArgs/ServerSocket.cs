using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// tcp socket 服务端
    /// </summary>
    public class ServerSocket : IWorkService, IDisposable
    {
        #region field ctor

        private readonly ConcurrentDictionary<ulong, Connection> connections = null;
        private readonly List<ResultEventHandler<OnReceivedSocketEventArgs, byte[]>> eventHandlers = null;
        private Socket socket = null;
        private readonly EndPoint listeningEndPoint = null;
        private readonly ISocketBufferProvider bufferProvider = null;
        private readonly SocketSetting setting;
        private readonly SocketAsyncEventArgs acceptSocketAsyncEventArgs = null;
        private readonly ISocketProtocol socketProtocol = null;
        private bool started = false;
        /// <summary>
        /// 
        /// </summary>
        public ServerSocket(SocketSetting setting, EndPoint listeningEndPoint, ISocketProtocol socketProtocol = null) : this(setting, new SocketBufferProvider(setting), listeningEndPoint, socketProtocol)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ServerSocket(SocketSetting setting, ISocketBufferProvider bufferProvider, EndPoint listeningEndPoint, ISocketProtocol socketProtocol = null)
        {
            this.listeningEndPoint = listeningEndPoint;
            this.bufferProvider = bufferProvider;
            this.setting = setting;
            this.socketProtocol = socketProtocol;
            this.eventHandlers = new List<ResultEventHandler<OnReceivedSocketEventArgs, byte[]>>();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = setting.ReceiveBufferSize,
                SendBufferSize = setting.SendBufferSize,
                NoDelay = true,
                Blocking = false,
            };

            this.connections = new ConcurrentDictionary<ulong, Connection>();
            this.acceptSocketAsyncEventArgs = new SocketAsyncEventArgs();
            this.acceptSocketAsyncEventArgs.Completed += AcceptAsyncCompleted;
        }

        #endregion

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

            this.Close();
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ServerSocket Start()
        {
            if (started)
                return this;

            this.started = true;
            this.socket.Bind(this.listeningEndPoint);
            this.socket.Listen(this.setting.MaxBacklog);
            this.StartAccept();
            return this;
        }

        private void StartAccept()
        {
            if (!this.socket.AcceptAsync(this.acceptSocketAsyncEventArgs))
                this.ProcessAccept(this, this.acceptSocketAsyncEventArgs);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public ServerSocket Close()
        {
            if (this.connections != null)
            {
                this.connections.UseForEach(ta => ta.Value.Dispose());
            }

            if (this.acceptSocketAsyncEventArgs != null)
            {
                this.acceptSocketAsyncEventArgs.Completed -= this.AcceptAsyncCompleted;
                this.acceptSocketAsyncEventArgs.AcceptSocket = null;
                this.acceptSocketAsyncEventArgs.Dispose();
            }

            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
            this.socket = null;
            return this;
        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        public event ResultEventHandler<OnReceivedSocketEventArgs, byte[]> OnMessageReceived
        {
            add
            {
                this.eventHandlers.Add(value);
            }
            remove
            {
                this.eventHandlers.Remove(value);
            }
        }

        /// <summary>
        /// 在连接关闭时刻
        /// </summary>
        public event EventHandler<SocketEventArgs> OnConnectionClosed;

        /// <summary>
        /// 在连接建立时刻
        /// </summary>
        public event EventHandler<SocketEventArgs> OnConnectionAccepted;

        private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(sender, e);
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            var acceptSocket = e.AcceptSocket;
            e.AcceptSocket = null;
            if (e.SocketError != SocketError.Success)
            {
                acceptSocket.Shutdown(SocketShutdown.Receive);
                acceptSocket.Dispose();
            }

            var connection = new Connection(acceptSocket, this.bufferProvider, socketProtocol);
            connection.OnMessageReceived += Connection_OnMessageReceived;
            connection.OnConnectionClosed += Connection_OnConnectionClosed;
            this.connections.TryAdd(connection.Id, connection);
            connection.Start();
            this.OnConnectionAccepted?.Invoke(this, new SocketEventArgs(connection) { });

            this.StartAccept();
        }

        private void Connection_OnConnectionClosed(object sender, SocketEventArgs e)
        {
            try
            {
                ObjectExtension.Raise(e, sender, ref this.OnConnectionClosed);
            }
            finally
            {
                this.connections.TryRemove(e.Connection.Id, out var cc);
            }
        }

        private IEnumerable<byte[]> Connection_OnMessageReceived(object sender, OnReceivedSocketEventArgs e)
        {
            foreach (var @delegate in this.eventHandlers)
            {
                try
                {
                    yield return @delegate(sender, e);
                }
                finally
                {

                }
            }
        }

        void IWorkService.Startup()
        {
            this.Start();
        }

        void IWorkService.Shutdown()
        {
            this.Close();
        }

        /// <summary>
        /// 所有的session
        /// </summary>
        public IEnumerable<IConnection> Sessions
        {
            get { return this.connections.Select(ta => ta.Value); }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sessionId">如果==0，则发送全部</param>
        /// <param name="data">消息</param>
        public void Push(ulong sessionId, byte[] data)
        {
            if (sessionId == 0)
            {
                var all = this.connections;
                if (all != null)
                {
                    foreach (var i in all)
                    {
                        i.Value.Push(data);
                    }
                }

                return;
            }

            var connection = this.connections.FirstOrDefault(ta => ta.Key == sessionId);
            if (connection.Value != null)
                connection.Value.Push(data);

            return;
        }
    }
}
