using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 缓冲区
    /// </summary>
    public struct SocketBufferProvider : ISocketBufferProvider
    {
        #region nested

        /// <summary>
        /// 缓冲区,分配的时候是按顺序位置
        /// </summary>
        private struct SocketBuffer : ISocketBuffer
        {
            public ArraySegment<byte> Segment { get; set; }

            public RangeTuple<int, int> Group { get; set; }
        }

        #endregion

        #region field and ctor

        private SocketSetting setting;
        private List<SocketBuffer[]> pools;
        private ConcurrentQueue<RangeTuple<int, int>> groups;

        /// <summary>
        /// 
        /// </summary>
        public SocketBufferProvider(SocketSetting setting) : this()
        {
            this.setting = setting;
            this.pools = new List<SocketBuffer[]>();
            this.groups = new ConcurrentQueue<RangeTuple<int, int>>();
            this.Extend();
        }

        #endregion

        #region prop

        /// <summary>
        /// 条数
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.setting.MaxBacklog * this.pools.Count;
            }
        }

        #endregion

        #region extend

        /// <summary>
        /// 扩展长度
        /// </summary>
        private void Extend()
        {
            var array = new byte[this.setting.MaxBacklog * this.setting.ReceiveBufferSize];
            var offset = 0;
            var count = pools.Count;
            var buffers = new SocketBuffer[setting.MaxBacklog];
            for (var i = 0; i < setting.MaxBacklog; i++)
            {
                var segment = new ArraySegment<byte>(array, offset, this.setting.ReceiveBufferSize);
                var buffer = new SocketBuffer()
                {
                    Group = new RangeTuple<int, int>(count, i),
                    Segment = segment,
                };
                buffers[i] = buffer;
                offset += this.setting.ReceiveBufferSize;
            }

            this.pools.Add(buffers);
            foreach (var buf in buffers)
            {
                this.groups.Enqueue(buf.Group);
            }
        }

        /// <summary>
        /// 扩展长度
        /// </summary>
        private void ExtendAgain()
        {
            //byte*[] array = new byte*[this.setting.MaxBacklog * this.setting.ReceiveBufferSize];
            //var offset = 0;
            //var count = pools.Count;
            //var buffers = new SocketBuffer[setting.MaxBacklog];
            //for (var i = 0; i < setting.MaxBacklog; i++)
            //{
            //    var segment = new ArraySegment<byte*>(array, offset, this.setting.ReceiveBufferSize);
            //    var buffer = new SocketBuffer()
            //    {
            //        Group = new RangeTuple<int, int>(count, i),
            //        Segment = segment,
            //    };
            //    buffers[i] = buffer;
            //    offset += this.setting.ReceiveBufferSize;
            //}

            //this.pools.Add(buffers);
            //foreach (var buf in buffers)
            //{
            //    this.groups.Enqueue(buf.Group);
            //}
        }
        #endregion

        #region provider

        /// <summary>
        /// 分配
        /// </summary>
        /// <returns></returns>
        public ISocketBuffer Alloc()
        {
            if (this.groups.TryDequeue(out var group))
            {
                return this.pools[group.Min][group.Max];
            }

            lock (pools)
            {
                if (this.groups.TryDequeue(out group))
                {
                    return this.pools[group.Min][group.Max];
                }

                this.Extend();
            }

            return this.Alloc();
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="item"></param>
        public void Recycle(ISocketBuffer item)
        {
            this.groups.Enqueue(item.Group);
        }

        #endregion
    }
}
