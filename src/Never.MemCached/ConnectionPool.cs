using Never.Memcached.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Never.Memcached
{
    /// <summary>
    /// 连接池
    /// </summary>
    public class ConnectionPool : IDisposable
    {
        #region nested

        /// <summary>
        /// 缓冲区,分配的时候是按顺序位置
        /// </summary>
        public struct ConnectionItem
        {
            public Connection Connection { get; set; }

            public RangeTuple<int, int> Group { get; set; }
        }

        #endregion

        #region field and ctor

        private SocketSetting setting;
        private List<ConnectionItem[]> pools;
        private ConcurrentQueue<RangeTuple<int, int>> groups;
        private Func<Connection> connectionInit;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionPool(SocketSetting setting, Func<Connection> connectionInit)
        {
            this.setting = setting;
            this.pools = new List<ConnectionItem[]>();
            this.groups = new ConcurrentQueue<RangeTuple<int, int>>();
            this.connectionInit = connectionInit;
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
                return this.pools.Count * this.setting.MaxPoolBufferSize;
            }
        }

        #endregion

        #region extend

        /// <summary>
        /// 扩展长度
        /// </summary>
        private void Extend()
        {
            var offset = 0;
            var count = this.pools.Count;
            var buffers = new ConnectionItem[this.setting.MaxPoolBufferSize];
            for (var i = 0; i < this.setting.MaxPoolBufferSize; i++)
            {
                var buffer = new ConnectionItem()
                {
                    Group = new RangeTuple<int, int>(count, i),
                    Connection = this.connectionInit(),
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

        #endregion

        #region provider

        /// <summary>
        /// 分配
        /// </summary>
        /// <returns></returns>
        public ConnectionItem Alloc()
        {
            if (this.groups.TryDequeue(out var group))
            {
                return this.pools[group.Min][group.Max];
            }

            lock (this.pools)
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
        public void Recycle(ConnectionItem item)
        {
            this.groups.Enqueue(item.Group);
        }


        #endregion

        #region dispose

        public void Dispose()
        {
            this.groups = null;
            foreach (var ps in this.pools)
            {
                foreach (var p in ps)
                {
                    try
                    {
                        p.Connection.Dispose();
                    }
                    finally
                    {

                    }
                }
            }
        }

        #endregion
    }
}
