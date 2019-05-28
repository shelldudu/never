using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Never.Threading;
using System.Net;
using Never.Utils;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class Connection : IConnection, IDisposable
    {
        #region field and ctor
        private Socket socket = null;
        private readonly ISocketBufferProvider bufferProvider = null;
        private readonly SocketAsyncEventArgs sendSocketAsyncEvent = null;
        private readonly SocketAsyncEventArgs receiveSocketAsyncEvent = null;
        private readonly InterLocker sendLocker = null;
        private readonly InterLocker receiveLocker = null;
        private readonly ConcurrentQueue<byte[]> sendDataQueue = null;
        private readonly ConcurrentQueue<byte[]> receiveDataQueue = null;
        private readonly ISocketProtocol dataProtocol = null;

        /// <summary>
        /// 
        /// </summary>
        public Connection(Socket socket, ISocketBufferProvider bufferProvider, ISocketProtocol socketProtocol = null)
        {
            this.socket = socket;
            this.LocalEndPoint = this.socket.LocalEndPoint;
            this.RemoteEndPoint = this.socket.RemoteEndPoint;
            this.ConnectTime = DateTime.Now;
            this.ProtocolType = this.socket.ProtocolType;
            this.bufferProvider = bufferProvider;

            //发送方根据得到的数据去setbuffer
            this.sendSocketAsyncEvent = new SocketAsyncEventArgs()
            {
                AcceptSocket = this.socket,
            };

            this.sendSocketAsyncEvent.Completed += SendSocketAsyncEvent_Completed;

            var buffer = this.bufferProvider.Alloc();
            this.receiveSocketAsyncEvent = new SocketAsyncEventArgs()
            {
                AcceptSocket = this.socket,
                UserToken = new SocketUserToken()
                {
                    SocketBuffer = buffer,
                },
            };

            this.receiveSocketAsyncEvent.Completed += ReceiveSocketAsyncEvent_Completed;
            this.receiveSocketAsyncEvent.SetBuffer(buffer.Segment.Array, buffer.Segment.Offset, buffer.Segment.Count);
            //注意这里塞入的是原始数据，拿出的再加工发送
            this.sendDataQueue = new ConcurrentQueue<byte[]>();
            //这里是获取了传输的数据，并不是buffer里面的
            this.receiveDataQueue = new ConcurrentQueue<byte[]>();
            this.sendLocker = new InterLocker();
            this.receiveLocker = new InterLocker();
            this.dataProtocol = socketProtocol ?? new DataProtocol();
            this.Id = NextId.Next();
        }

        #endregion

        #region send

        private void SendSocketAsyncEvent_Completed(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessSend(sender, e);
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            //有别的任务是发送数据
            if (this.sendLocker.CanLock)
                this.TryProcessSend(sender, e);

            //为什么不用这样的代码，因为当这一刻不能拿到锁，说明有其他线程在处理，而这个处理有可能是处理中，或者在退出锁中，
            //在退出锁中这一种情况就会引起当前线程TA由于return了，而处理退出锁的TB等下又不会触发发送行为
            //if (!this.sendLocker.CanLock)
            //    return;
            //this.TryProcessSend(sender, e);
        }

        private void TryProcessSend(object sender, SocketAsyncEventArgs e)
        {
            this.sendLocker.TryEnterLock(() =>
            {
                //从缓冲池那里获取数据
                while (this.sendDataQueue.TryDequeue(out var bytes))
                {
                    var a = this.dataProtocol.To(bytes);
                    e.SetBuffer(a, 0, a.Length);
                    if (!e.AcceptSocket.SendAsync(e))
                    {
                        //由于有锁，本身占用了锁，所以只能开异步去处理，但会不会造成太多线程呢？
                        //this.ProcessSend(this, e);
                    }
                }
            }, true);
        }

        #endregion send

        #region receive

        private void ReceiveSocketAsyncEvent_Completed(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessReceive(sender, e);
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                this.CloseSocket(e.SocketError, null);
                return;
            }

            //这里有可能粘包
            var receive = new byte[e.BytesTransferred];
            Buffer.BlockCopy(e.Buffer, e.Offset, receive, 0, e.BytesTransferred);
            this.receiveDataQueue.Enqueue(receive);

            //有别的任务是处理数据
            if (this.receiveLocker.CanLock)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    this.TryProcessReceive(sender, e);
                });
            }

            if (!e.AcceptSocket.ReceiveAsync(e))
                this.ProcessReceive(sender, e);

            //为什么不用这样的代码，因为当这一刻不能拿到锁，说明有其他线程在处理，而这个处理有可能是处理中，或者在退出锁中，
            //在退出锁中这一种情况就会引起当前线程TA由于return了，而处理退出锁的TB等下又不会触发发送行为
            //if (!this.receiveLocker.CanLock)
            //{
            //    if (!e.AcceptSocket.ReceiveAsync(e))
            //        this.ProcessReceive(sender, e);

            //    return;
            //}
        }

        private void TryProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            this.receiveLocker.TryEnterLock(() =>
            {
                if (this.OnMessageReceived == null)
                    return;

                //处理粘包
                var data = this.dataProtocol.From(this.receiveDataQueue);
                if (data == null)
                    return;

                var buffer = this.OnMessageReceived.Invoke(this, new OnReceivedSocketEventArgs(this, data)).ToArray();
                this.Push(buffer);
            }, true);
        }

        #endregion

        #region start

        /// <summary>
        /// 
        /// </summary>
        public virtual void Start()
        {
            this.ProcessSend(this, this.sendSocketAsyncEvent);

            if (!this.receiveSocketAsyncEvent.AcceptSocket.ReceiveAsync(this.receiveSocketAsyncEvent))
                this.ProcessReceive(this, this.receiveSocketAsyncEvent);
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
        public bool IsConnected { get { return this.socket != null && this.socket.Connected; } }

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

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Push(byte[] data)
        {
            if (data != null)
            {
                this.sendDataQueue.Enqueue(data);
            }

            if (this.sendLocker.CanLock)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    this.ProcessSend(this, this.sendSocketAsyncEvent);
                });
            }

            //不使用if (!this.sendLocker.CanLock)的原理在上面有说到
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="array"></param>
        public void Push(IEnumerable<byte[]> array)
        {
            if (array != null)
            {
                foreach (var data in array)
                {
                    if (data != null)
                        this.sendDataQueue.Enqueue(data);
                }
            }

            if (this.sendLocker.CanLock)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    this.ProcessSend(this, this.sendSocketAsyncEvent);
                });
            }
            //不使用if (!this.sendLocker.CanLock)的原理在上面有说到
        }

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
                if (this.sendSocketAsyncEvent != null)
                {
                    this.sendSocketAsyncEvent.Completed -= this.SendSocketAsyncEvent_Completed;
                    this.sendSocketAsyncEvent.AcceptSocket = null;
                    this.sendSocketAsyncEvent.Dispose();
                }
            }
            finally
            {

            }
            try
            {
                if (this.receiveSocketAsyncEvent != null)
                {
                    this.bufferProvider.Recycle(((SocketUserToken)this.receiveSocketAsyncEvent.UserToken).SocketBuffer);
                    this.receiveSocketAsyncEvent.Completed -= this.ReceiveSocketAsyncEvent_Completed;
                    this.receiveSocketAsyncEvent.AcceptSocket = null;
                    this.receiveSocketAsyncEvent.Dispose();
                }
            }
            finally
            {

            }
            try
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
            }
            finally
            {
                this.socket = null;
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

            if (this.socket == null)
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
                socket.IOControl(IOControlCode.KeepAliveValues, inOptionValue, outOptionValue);
                return true;
            }
            catch (NotSupportedException)
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (NotImplementedException)
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValue);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region protocol

        /// <summary>
        /// 数据报文协议解析
        /// </summary>
        public class DataProtocol : ISocketProtocol
        {
            /// <summary>
            /// 转换成转输的数组
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public byte[] To(byte[] data)
            {
                var length = data.Length;
                var s1 = BitConverter.GetBytes(length);
                var s2 = data;
                var s = ObjectExtension.Combine(s1, s2);
                return s;
            }

            //上一次读取后还没处理的数据
            private int lastPosition = 0;
            private byte[] lastData = null;

            //数据的长度
            private int lengthPosition = 0;
            private byte[] lengthData = null;

            //目标的长度
            private int targetPosition = 0;
            private byte[] targetData = null;

            /// <summary>
            /// 解析数据，byte[0-4]表示数据长度，如果如果当前容量长度n小于4，表示要等下一波数据来临后读取4-n位来确定这一波数据长度，
            /// 当读取到长度后l，再读取后面的数据，只有读取到l长度的数据后表明这个数据可以处理（可开一个异步去处理），此时下一个数据
            /// 的始点是从l+1开始的，如此循环
            /// </summary>
            /// <param name="collection"></param>
            /// <returns></returns>
            public byte[] From(ConcurrentQueue<byte[]> collection)
            {
                if (collection.IsEmpty)
                    return null;

                if (this.lengthData == null)
                    this.lengthData = new byte[4];

                int start;
                if (this.lengthPosition == 4)
                    goto _find;

                //先读取4位，确定数值长度，长度有可能在lastdate也有可能在nextdate,即便在lastdate，但数据也有可能在next的或者在next.next.next等
                while (true)
                {
                    //在上一次字节占还有数据还没读完
                    if (this.lastData != null)
                    {
                        for (start = this.lastPosition; start < this.lastData.Length; start++)
                        {
                            this.lengthData[this.lengthPosition] = this.lastData[start];
                            this.lastPosition++;
                            this.lengthPosition++;
                            if (this.lengthPosition == 4)
                                goto _find;
                        }
                    }

                    //此时的this.lastdate一定是读取完了
                    while (collection.TryDequeue(out lastData))
                    {
                        this.lastPosition = 0;
                        for (start = 0; start < this.lastData.Length; start++)
                        {
                            this.lengthData[this.lengthPosition] = this.lastData[start];
                            this.lastPosition++;
                            this.lengthPosition++;
                            if (this.lengthPosition == 4)
                                goto _find;
                        }
                    }

                    return null;
                }

            _find:
                {
                    if (this.lastData != null && this.lastData.Length == this.lastPosition)
                    {
                        this.lastPosition = 0;
                        this.lastData = null;
                    }

                    if (this.targetData == null)
                    {
                        var length = BitConverter.ToInt32(this.lengthData, 0);
                        this.targetData = new byte[length];
                        //if (length == 0)
                        //{
                        //    this.targetPosition = 0;
                        //    return targetData;
                        //}
                    }

                    for (var i = this.targetPosition; i < this.targetData.Length; i++)
                    {
                        //在上一次字节占还有数据还没读完
                        if (this.lastData != null)
                        {
                            for (start = this.lastPosition; start < this.lastData.Length; start++)
                            {
                                this.targetData[this.targetPosition] = this.lastData[start];
                                this.lastPosition++;
                                this.targetPosition++;
                                if (this.targetPosition == this.targetData.Length)
                                    goto _done;
                            }
                        }

                        //此时的this.lastdate一定是读取完了
                        while (collection.TryDequeue(out lastData))
                        {
                            this.lastPosition = 0;
                            for (start = 0; start < this.lastData.Length; start++)
                            {
                                this.targetData[this.targetPosition] = this.lastData[start];
                                this.lastPosition++;
                                this.targetPosition++;
                                if (this.targetPosition == this.targetData.Length)
                                    goto _done;
                            }
                        }
                    }

                    return null;
                }
            _done:
                {
                    if (this.lastData != null && this.lastData.Length == this.lastPosition)
                    {
                        this.lastPosition = 0;
                        this.lastData = null;
                    }
                    if (this.lengthData != null && this.lengthData.Length == this.lengthPosition)
                    {
                        this.lengthPosition = 0;
                        //if (BitConverter.ToInt32(this.lengthData, 0) == 0)
                        //{
                        //    this.targetPosition = 0;
                        //    this.targetData = null;
                        //    return new byte[0];
                        //}
                    }
                    var target = this.targetData;
                    this.targetPosition = 0;
                    this.targetData = null;
                    return target;
                }
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
