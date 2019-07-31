using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Never.Logging;

namespace Never.Memcached
{
    /// <summary>
    /// 文本协议
    /// </summary>
    public class TextCached : MemcachedClient
    {
        #region field and ctor

        private readonly ConnectionPool[] connectionPools = null;
        private readonly EndPoint[] servers = null;
        private readonly Encoding responseEncoding = null;
        private readonly ICompressProtocol compress = null;

        public TextCached(EndPoint[] servers, ICompressProtocol compress) : this(servers, Encoding.UTF8, compress)
        {
        }

        public TextCached(EndPoint[] servers, Encoding responseEncoding, ICompressProtocol compress) : this(servers, responseEncoding, compress, new SocketSetting())
        {
        }

        public TextCached(EndPoint[] servers, Encoding responseEncoding, ICompressProtocol compress, SocketSetting setting) : base(compress)
        {
            this.servers = servers;
            this.responseEncoding = responseEncoding;
            this.compress = compress;
            this.connectionPools = new ConnectionPool[servers.Length];
            for (var i = 0; i < servers.Length; i++)
            {
                var connectionPool = new ConnectionPool(setting, servers[i], (set, end) =>
                {
                    var client = new Sockets.ClientSocket(set, end);
                    client.Start().KeepAlive(set.KeepAlivePeriod);
                    return client.Connection;
                });

                this.connectionPools[i] = connectionPool;
            }
        }

        #endregion field and ctor

        #region prop

        /// <summary>
        /// 写日志的
        /// </summary>
        public Func<ILoggerBuilder> LoggerBuilder { get; set; }

        #endregion prop

        #region set add replace delete interlocked

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        protected override bool TryAddValue(string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            //我们发现add,set,replace的行为是一样的
            return this.TrySetValue(Command.add, key, value, flag, expireTime);
        }

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        protected override bool TrySetValue(string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            //我们发现add,set,replace的行为是一样的
            return this.TrySetValue(Command.set, key, value, flag, expireTime);
        }

        /// <summary>
        /// 尝试替换一个key并指定过期时间
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        protected override bool TryReplaceValue(string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            //我们发现add,set,replace的行为是一样的
            return this.TrySetValue(Command.replace, key, value, flag, expireTime);
        }

        /// <summary>
        /// 尝试在此之后新加一个key并指定过期时间
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        protected override bool TryAppendValue(string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            return this.TrySetValue(Command.append, key, value, flag, expireTime);
        }

        /// <summary>
        /// 尝试在此之前新加一个key并指定过期时间
        /// </summary>
        /// <param name="command"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        protected override bool TryPrependValue(string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            return this.TrySetValue(Command.prepend, key, value, flag, expireTime);
        }

        /// <summary>
        /// 开始set
        /// </summary>
        protected bool TrySetValue(Command command, string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            if (expireTime < TimeSpan.Zero)
                return true;

            if (command < Command.get || command > Command.saslstep)
                return false;

            if (key.IsNullOrWhiteSpace())
                return false;

            var hv = this.SumASCII(key);
            var sort = this.SortServer(hv, this.connectionPools.Length);
            foreach (var s in sort)
            {
                if (dodo(this.connectionPools[s]))
                    return true;
            }

            return false;

            //开始set
            bool dodo(ConnectionPool pool)
            {
                var request = string.Concat(command.ToString(), " ", key, " ", flag, " ", expireTime.TotalSeconds, " ", value.Length, "\r\n");
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(Encoding.ASCII.GetBytes(request));
                    item.Connection.Write(value, 0, value.Length);
                    item.Connection.Write(Encoding.ASCII.GetBytes("\r\n"));
                    item.Connection.Flush();

                    var line = item.Connection.ReadLine(this.responseEncoding);
                    switch (line)
                    {
                        case "STORED":
                            {
                                return true;
                            }
                        case "EXISTS":
                            {
                                //主要是这里引出不同
                                return command == Command.add ? false : true;
                            }
                        case "NOT_STORED":
                            {
                                if (this.LoggerBuilder == null)
                                    return false;

                                this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat(command, "key", key, "not stored"));
                                return false;
                            }
                        default:
                            {
                                return false;
                            }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    pool.Recycle(item);
                }
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public override bool TryGetValue(string key, out object value)
        {
            value = null;
            if (string.IsNullOrEmpty(key))
                return false;

            var hv = this.SumASCII(key);
            var sort = this.SortServer(hv, this.connectionPools.Length);
            foreach (var s in sort)
            {
                if (dodo(this.connectionPools[s], out value))
                    return true;
            }

            return false;

            bool dodo(ConnectionPool pool, out object val)
            {
                val = null;
                var request = string.Concat("get ", key, "\r\n");
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(Encoding.ASCII.GetBytes(request));
                    item.Connection.Flush();
                    byte[] valueByte = default(byte[]); string nkey = string.Empty; byte nflag = 0; int nlength = 0;

                    do
                    {
                        var line = item.Connection.ReadLine(this.responseEncoding);
                        if (line == null)
                            continue;

                        //第二个是读取interlocked
                        if (line == "END" || line == "\0\0END")
                        {
                            if (valueByte != null)
                            {
                                if (this.compress.TryDecompress(valueByte, nflag, out val))
                                    return true;

                                return false;
                            }

                            return false;
                        }

                        if (line.Length < 5 || line[0] != 'V' || line[1] != 'A' || line[2] != 'L' || line[3] != 'U' || line[4] != 'E')
                            continue;

                        //find the flag : VALUE <key> <flags> <bytes> [<cas unique>]
                        var splits = line.Split(' ');
                        nkey = splits[1];
                        nflag = splits[2].AsByte();
                        nlength = splits[3].AsInt();
                        valueByte = item.Connection.Read(nlength);
                    } while (true);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    pool.Recycle(item);
                }
            }
        }

        /// <summary>
        /// 获取对象，不想装箱与拆箱
        /// </summary>
        public override bool TryGetResult<T>(string key, out T value)
        {
            value = default(T);
            if (string.IsNullOrEmpty(key))
                return false;

            var hv = this.SumASCII(key);
            var sort = this.SortServer(hv, this.connectionPools.Length);
            foreach (var s in sort)
            {
                if (dodo(this.connectionPools[s], out value))
                    return true;
            }

            return false;

            bool dodo(ConnectionPool pool, out T val)
            {
                val = default(T);
                var request = string.Concat("get ", key, "\r\n");
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(Encoding.ASCII.GetBytes(request));
                    item.Connection.Flush();
                    byte[] valueByte = default(byte[]); string nkey = string.Empty; byte nflag = 0; int nlength = 0;
                    do
                    {
                        var line = item.Connection.ReadLine(this.responseEncoding);
                        if (line == null)
                            continue;

                        //第二个是读取interlocked
                        if (line == "END" || line == "\0\0END")
                        {
                            if (valueByte != null)
                            {
                                if (this.compress.TryDecompress<T>(valueByte, nflag, out val))
                                    return true;

                                return false;
                            }

                            return false;
                        }

                        if (line.Length < 5 || line[0] != 'V' || line[1] != 'A' || line[2] != 'L' || line[3] != 'U' || line[4] != 'E')
                            continue;

                        //find the flag : VALUE <key> <flags> <bytes> [<cas unique>]
                        var splits = line.Split(' ');
                        nkey = splits[1];
                        nflag = splits[2].AsByte();
                        nlength = splits[3].AsInt();
                        valueByte = item.Connection.Read(nlength);
                    } while (true);
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    pool.Recycle(item);
                }
            }
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <returns></returns>
        public override bool TryDelete(string key)
        {
            if (key.IsNullOrWhiteSpace())
                return false;

            var hv = this.SumASCII(key);
            var sort = this.SortServer(hv, this.connectionPools.Length);
            foreach (var s in sort)
            {
                if (dodo(this.connectionPools[s]))
                    return true;
            }

            return false;

            //开始something
            bool dodo(ConnectionPool pool)
            {
                var request = string.Concat("delete ", key, "\r\n");
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(Encoding.ASCII.GetBytes(request));
                    item.Connection.Flush();

                    var line = item.Connection.ReadLine(this.responseEncoding);
                    if (line == null)
                        return false;

                    switch (line)
                    {
                        case "NOT_FOUND":
                            {
                                if (this.LoggerBuilder == null)
                                    return false;

                                this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("delete key", key, "not found"));
                                return false;
                            }
                        case "DELETED":
                            {
                                return true;
                            }
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    pool.Recycle(item);
                }
            }
        }

        /// <summary>
        /// 对值+-1
        /// </summary>
        /// <returns></returns>
        protected override bool TryInterlocked(Command command, string key, long inter, TimeSpan expireTime)
        {
            var commandName = command == Command.increment ? "incr" : (command == Command.decrement ? "decr" : "");
            if (commandName.IsNullOrWhiteSpace())
                return false;

            if (key.IsNullOrWhiteSpace())
                return false;

            var hv = this.SumASCII(key);
            var sort = this.SortServer(hv, this.connectionPools.Length);
            foreach (var s in sort)
            {
                if (dodo(this.connectionPools[s]))
                    return true;
            }

            return false;

            //开始something
            bool dodo(ConnectionPool pool)
            {
                var request = string.Concat(commandName, " ", key, " ", inter, "\r\n");
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(Encoding.ASCII.GetBytes(request));
                    item.Connection.Flush();

                    var line = item.Connection.ReadLine(this.responseEncoding);
                    if (line == null)
                        return false;

                    if (line == "NOT_FOUND")
                    {
                        if (this.LoggerBuilder == null)
                            return false;

                        this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("decr/incr key", key, "not found"));
                        return false;
                    }

                    if (this.IsDigit(line))
                        return true;

                    return false;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    pool.Recycle(item);
                }
            }
        }

        #endregion set add replace delete interlocked

        #region dispose

        /// <summary>
        ///
        /// </summary>
        /// <param name="dispose"></param>
        protected override void Dispose(bool dispose)
        {
            foreach (var pool in this.connectionPools)
            {
                pool.Dispose();
            }

            base.Dispose(dispose);
        }

        #endregion dispose
    }
}