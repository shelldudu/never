using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Never.Threading;
using Never.Utils;

namespace Never.Memcached.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public class Connection : IConnection, IDisposable
    {
        #region field and ctor

        private readonly InterLocker sendLocker = null;
        private readonly InterLocker receiveLocker = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        public Connection(Socket socket, Func<Socket, Stream> stream) : this(socket, new BufferedStream(stream(socket)))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        public Connection(Socket socket, Stream stream)
        {
            this.Socket = socket;
            this.Stream = stream;
            this.LocalEndPoint = this.Socket.LocalEndPoint;
            this.RemoteEndPoint = this.Socket.RemoteEndPoint;
            this.ConnectTime = DateTime.Now;
            this.ProtocolType = this.Socket.ProtocolType;
            this.sendLocker = new InterLocker();
            this.receiveLocker = new InterLocker();
            this.Id = NextId.Next();
        }

        #endregion

        #region prop

        /// <summary>
        /// socket
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// stream
        /// </summary>
        public Stream Stream { get; private set; }

        #endregion

        #region write

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Write(byte[] bytes)
        {
            this.Stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Write(IEnumerable<byte[]> bytes)
        {
            foreach (var @byte in bytes)
                this.Stream.Write(@byte, 0, @byte.Length);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public virtual void Write(byte[] bytes, int offset, int count)
        {
            this.Stream.Write(bytes, offset, count);
        }

        /// <summary>
        /// 每写一次都要重新写入流中
        /// </summary>
        public virtual void Flush()
        {
            this.Stream.Flush();
        }

        #endregion write

        #region read

        /// <summary>
        /// 读取一行，但是返回的结果是包含了换行或者回车
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ReadLine()
        {
            var @byte = new byte[1];
            var end = false;
            using (var st = new MemoryStream())
            {
                while (this.Stream.Read(@byte, 0, 1) != -1)
                {
                    if (@byte[0] == 13)
                    {
                        end = true;

                    }
                    else
                    {
                        if (end)
                        {
                            if (@byte[0] == 10)
                                break;

                            end = false;
                        }
                    }

                    st.Write(@byte, 0, 1);
                }

                if (st.Length <= 0)
                    return null;

                return st.GetBuffer();
            }
        }

        /// <summary>
        /// 读取一行
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public virtual string ReadLine(Encoding encoding)
        {
            var @byte = new byte[1];
            var end = false;
            using (var st = new MemoryStream())
            {
                while (this.Stream.Read(@byte, 0, 1) != -1)
                {
                    if (@byte[0] == 13)
                    {
                        end = true;

                    }
                    else
                    {
                        if (end)
                        {
                            if (@byte[0] == 10)
                                break;

                            end = false;
                        }
                    }

                    st.Write(@byte, 0, 1);
                }

                if (st.Length <= 0)
                    return null;

                return encoding.GetString(st.GetBuffer()).TrimEnd(new[] { '\0', '\r', '\n' });
            }
        }

        /// <summary>
        /// 清空结束换行
        /// </summary>
        public virtual void ClearLine()
        {
            var @byte = new byte[1];
            var end = false;
            while (this.Stream.Read(@byte, 0, 1) != -1)
            {
                //13是回车 \r
                if (@byte[0] == 13)
                {
                    end = true;
                    continue;
                }

                if (end)
                {
                    //10是换行\n
                    if (@byte[0] == 10)
                        break;

                    end = false;
                }
            }
        }

        /// <summary>
        /// 读字节
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] Read(int length)
        {
            var @byte = new byte[length];
            var count = 0;
            while (count < length)
            {
                var cnt = this.Stream.Read(@byte, count, length - count);
                if (cnt <= 0)
                    return @byte;

                count += cnt;
            }

            return @byte;
        }

        /// <summary>
        /// 尝试清空
        /// </summary>
        public virtual void ClearStream()
        {
            this.Stream.Flush();
            var ava = this.Socket.Available;
            if (ava > 0)
            {
                this.Read(ava);
            }
        }

        #endregion read

        #region start

        /// <summary>
        /// 
        /// </summary>
        public virtual void Start()
        {
        }

        #endregion

        #region isession

        /// <summary>
        /// 会话Id
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// 获取本机终结点
        /// </summary>
        public EndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectTime { get; }

        /// <summary>
        /// 支持的协议
        /// </summary>
        public ProtocolType ProtocolType { get; }

        /// <summary>
        /// 是否连接了
        /// </summary>
        public bool IsConnected { get { return this.Socket != null && this.Socket.Connected; } }

        /// <summary>
        /// 是否为安全连接的
        /// </summary>
        public virtual bool IsSecurity { get; }

        #endregion

        #region handle

        /// <summary>
        /// 接收到数据
        /// </summary>
        internal Func<object, OnReceivedSocketEventArgs, IEnumerable<byte[]>> OnMessageReceived { get; set; }

        /// <summary>
        /// 在连接关闭时刻
        /// </summary>
        internal Action<object, OnClosedSocketEventArgs> OnConnectionClosed { get; set; }

        #endregion

        #region close

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.CloseSocket();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="exception"></param>
        protected void CloseSocket(SocketError error, Exception exception)
        {
            try
            {
                this.OnConnectionClosed?.Invoke(this, new OnClosedSocketEventArgs(this, error, exception));
            }
            finally
            {

            }

            this.CloseSocket();
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        protected void CloseSocket()
        {
            try
            {
                this.Stream.Flush();
                this.Stream = null;
            }
            finally
            {

            }
            try
            {
                this.Socket.Shutdown(SocketShutdown.Both);
                this.Socket.Close();
            }
            finally
            {
                this.Socket = null;
            }
        }

        #endregion

        #region keep alive

        /// <summary>
        /// 设置心跳包
        /// </summary>
        /// <param name="keepAlivePeriod">时间间隔</param>
        /// <returns></returns>
        public bool KeepAlive(TimeSpan keepAlivePeriod)
        {
            if (keepAlivePeriod == TimeSpan.Zero)
            {
                return false;
            }

            if (this.Socket == null)
            {
                return false;
            }

            var period = (int)keepAlivePeriod.TotalMilliseconds;

            var inOptionValue = new byte[12];
            var outOptionValue = new byte[12];
            BitConverter.GetBytes(1).CopyTo(inOptionValue, 0);
            BitConverter.GetBytes(period).CopyTo(inOptionValue, 4);
            BitConverter.GetBytes(period).CopyTo(inOptionValue, 8);
            try
            {
                this.Socket.IOControl(IOControlCode.KeepAliveValues, inOptionValue, outOptionValue);
                return true;
            }
            catch (NotSupportedException)
            {
                this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (NotImplementedException)
            {
                this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region nextid

        /// <summary>
        /// Id生成
        /// </summary>
        public struct NextId
        {
            private static int increment = 0;
            private static int channelId = 5;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static ulong Next()
            {
                var a = System.Threading.Interlocked.Increment(ref increment);
                if (a == int.MaxValue)
                {
                    System.Threading.Interlocked.Increment(ref channelId);
                }

                return (ulong)Channel.GetNewsId(a, channelId);
            }
        }

        #endregion
    }
}
