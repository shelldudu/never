using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Never;
using Never.Logging;

namespace Never.Memcached
{
    /// <summary>
    /// 二进制协议
    /// </summary>
    public sealed class BinaryCached : MemcachedClient
    {
        #region field and ctor
        private readonly ConnectionPool[] connectionPools = null;
        private readonly EndPoint[] servers = null;
        private readonly Encoding responseEncoding = null;
        private readonly ICompressProtocol compress = null;
        public BinaryCached(EndPoint[] servers, ICompressProtocol compress) : this(servers, Encoding.UTF8, compress)
        {
        }

        public BinaryCached(EndPoint[] servers, Encoding responseEncoding, ICompressProtocol compress) : this(servers, responseEncoding, compress, new SocketSetting())
        {
        }

        public BinaryCached(EndPoint[] servers, Encoding responseEncoding, ICompressProtocol compress, SocketSetting setting) : base(compress)
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

        #endregion

        #region prop

        /// <summary>
        /// 写日志的
        /// </summary>
        public Func<ILoggerBuilder> LoggerBuilder { get; set; }

        #endregion

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
        private bool TrySetValue(Command command, string key, byte[] value, byte flag, TimeSpan expireTime)
        {
            if (expireTime < TimeSpan.Zero)
                return true;

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
                var request = BinaryConvert.AddRequest(command, key, value, flag, this.GetTotalSecond(expireTime));
                var item = pool.Alloc();

                try
                {
                    item.Connection.Write(request);
                    item.Connection.Flush();
                    var @byte = item.Connection.Read(24);
                    var status = BinaryConvert.ParseStatus(@byte);
                    switch ((BinaryProtocols.ResponseStatus)status)
                    {
                        case BinaryProtocols.ResponseStatus.NoError:
                            {
                                return true;
                            }
                        case BinaryProtocols.ResponseStatus.KeyExists:
                            {
                                //主要是这里引出不同
                                return command == Command.add ? false : true;
                            }
                        case BinaryProtocols.ResponseStatus.KeyNotFound:
                            {
                                if (this.LoggerBuilder == null)
                                    return false;

                                this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat(command, "key", key, "not stored"));
                                return false;
                            }
                        default:
                            {
                                if (this.LoggerBuilder != null)
                                    this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat(command, "key", key, "status:", status));

                                return false;
                            }
                    }
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
                var request = BinaryConvert.GetRequest(key);
                var item = pool.Alloc();
                var detory = false;
                try
                {
                    item.Connection.Write(request);
                    item.Connection.Flush();
                    var @byte = item.Connection.Read(24);
                    var status = BinaryConvert.ParseStatus(@byte);
                    var nflag = byte.MinValue;
                    var totallength = BinaryConvert.ParseTotalLength(@byte);
                    var extralength = BinaryConvert.ParseExtraLength(@byte);
                    var keylength = BinaryConvert.ParseKeyLength(@byte);
                    if (status != 0x0000)
                    {
                        if (totallength > 0)
                            item.Connection.Read(totallength);

                        return false;
                    }

                    var alldata = item.Connection.Read(totallength);
                    var offset = 0;
                    if (extralength > 0)
                    {
                        offset += 4;
                        nflag = alldata[3];
                    }

                    var nkey = string.Empty;
                    if (keylength > 0)
                    {
                        var data = new byte[keylength];
                        Buffer.BlockCopy(alldata, 0, data, offset, data.Length);
                        offset += data.Length;
                        nkey = Encoding.ASCII.GetString(data);
                    }

                    if (totallength > 0)
                    {
                        var data = new byte[totallength - offset];
                        Buffer.BlockCopy(alldata, offset, data, 0, data.Length);
                        offset += data.Length;
                        if (this.compress.TryDecompress(data, nflag, out val))
                            return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    detory = true;
                    return false;
                }
                finally
                {
                    if (detory)
                        pool.Detory(item);
                    else
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
                var request = BinaryConvert.GetRequest(key);
                var item = pool.Alloc();
                var detory = false;
                try
                {
                    item.Connection.Write(request);
                    item.Connection.Flush();
                    var @byte = item.Connection.Read(24);
                    var status = BinaryConvert.ParseStatus(@byte);
                    var nflag = byte.MinValue;
                    var totallength = BinaryConvert.ParseTotalLength(@byte);
                    var extralength = BinaryConvert.ParseExtraLength(@byte);
                    var keylength = BinaryConvert.ParseKeyLength(@byte);
                    if (status != 0x0000)
                    {
                        if (totallength > 0)
                        {
                            item.Connection.Read(totallength);
                        }
                        return false;
                    }

                    var alldata = item.Connection.Read(totallength);
                    var offset = 0;
                    if (extralength > 0)
                    {
                        offset += 4;
                        nflag = alldata[3];
                    }

                    var nkey = string.Empty;
                    if (keylength > 0)
                    {
                        var data = new byte[keylength];
                        Buffer.BlockCopy(alldata, 0, data, offset, data.Length);
                        offset += data.Length;
                        nkey = Encoding.ASCII.GetString(data);
                    }

                    if (totallength > 0)
                    {
                        var data = new byte[totallength - offset];
                        Buffer.BlockCopy(alldata, offset, data, 0, data.Length);
                        offset += data.Length;
                        if (this.compress.TryDecompress(data, nflag, out val))
                            return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    detory = true;
                    return false;
                }
                finally
                {
                    if (detory)
                        pool.Detory(item);
                    else
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
                var request = BinaryConvert.DeleteRequest(key);
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(request);
                    item.Connection.Flush();
                    var @byte = item.Connection.Read(24);
                    var status = BinaryConvert.ParseStatus(@byte);
                    switch ((BinaryProtocols.ResponseStatus)status)
                    {
                        case BinaryProtocols.ResponseStatus.KeyExists:
                            {
                                if (this.LoggerBuilder == null)
                                    return false;

                                this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("delete key", key, "not found"));
                                return false;
                            }
                        case BinaryProtocols.ResponseStatus.NoError:
                            {
                                return true;
                            }
                        default:
                            {
                                if (this.LoggerBuilder != null)
                                    this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("delete key", "key", key, "status:", status));

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
        /// 对值+-1
        /// </summary>
        /// <returns></returns>
        protected override bool TryInterlocked(Command command, string key, long inter, TimeSpan expireTime)
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
                var request = BinaryConvert.InterlockedRequest(command, key, inter, this.GetTotalSecond(expireTime));
                var item = pool.Alloc();
                try
                {
                    item.Connection.Write(request);
                    item.Connection.Flush();
                    var @byte = item.Connection.Read(24);
                    var status = BinaryConvert.ParseStatus(@byte);
                    switch ((BinaryProtocols.ResponseStatus)status)
                    {
                        case BinaryProtocols.ResponseStatus.KeyExists:
                            {
                                if (this.LoggerBuilder == null)
                                    return false;

                                this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("decr/incr key", key, "not found"));
                                return false;
                            }
                        case BinaryProtocols.ResponseStatus.NoError:
                            {
                                return true;
                            }
                        default:
                            {
                                if (this.LoggerBuilder != null)
                                    this.LoggerBuilder().Build(typeof(TextCached)).Info(string.Concat("decr/incr key", "key", key, "status:", status));

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

        #endregion

        #region bitconvert

        public class BinaryConvert
        {
            public static short ParseStatus(byte[] @byte)
            {
                return (short)(@byte[7] | @byte[6] << 8);
            }

            public static int ParseTotalLength(byte[] @byte)
            {
                return @byte[11] | @byte[10] << 8 | @byte[9] << 16 | @byte[8] << 24;
            }

            public static byte ParseExtraLength(byte[] @byte)
            {
                return @byte[4];
            }

            public static short ParseKeyLength(byte[] @byte)
            {
                return (short)(@byte[3] | @byte[2] << 8);
            }

            public static IEnumerable<byte[]> AddRequest(Command command, string key, byte[] body, byte flag, int expireTime)
            {
                var keyData = Encoding.ASCII.GetBytes(key);
                var extraData = new byte[8];
                var headerData = new byte[24];
                var totalLength = extraData.Length + keyData.Length + body.Length;

                extraData[0] = 0;
                extraData[1] = 0;
                extraData[2] = 0;
                extraData[3] = flag;
                extraData[4] = (byte)(expireTime >> 24);
                extraData[5] = (byte)(expireTime >> 16);
                extraData[6] = (byte)(expireTime >> 8);
                extraData[7] = (byte)(expireTime);

                //magic
                headerData[0] = 0x80;
                //opcode
                headerData[1] = (byte)command;
                //keylength
                headerData[2] = (byte)(keyData.Length >> 8);
                headerData[3] = (byte)(keyData.Length);
                //extralength
                headerData[4] = (byte)(extraData.Length);
                //datatype
                headerData[5] = 0;
                //vbucket
                headerData[6] = 0;
                headerData[7] = 0;

                //totalbody
                headerData[8] = (byte)(totalLength >> 24);
                headerData[9] = (byte)(totalLength >> 16);
                headerData[10] = (byte)(totalLength >> 8);
                headerData[11] = (byte)(totalLength);

                //opaque
                headerData[12] = headerData[13] = headerData[14] = headerData[15] = 0;

                //cas
                headerData[16] = headerData[17] = headerData[18] = headerData[19] = headerData[20] = headerData[21] = headerData[22] = headerData[23] = 0;
                var @enumerable = new[] { headerData, extraData, keyData, body }.ToArray();
                return @enumerable;
            }

            public static IEnumerable<byte[]> DeleteRequest(string key)
            {
                var keyData = Encoding.ASCII.GetBytes(key);
                var headerData = new byte[24];
                var totalLength = keyData.Length;
                //magic
                headerData[0] = 0x80;
                //opcode
                headerData[1] = (byte)Command.delete;
                //keylength
                headerData[2] = (byte)(keyData.Length >> 8);
                headerData[3] = (byte)(keyData.Length);
                //extralength
                headerData[4] = 0;
                //datatype
                headerData[5] = 0;
                //vbucket
                headerData[6] = 0;
                headerData[7] = 0;

                //totalbody
                headerData[8] = (byte)(totalLength >> 24);
                headerData[9] = (byte)(totalLength >> 16);
                headerData[10] = (byte)(totalLength >> 8);
                headerData[11] = (byte)(totalLength);

                //opaque
                headerData[12] = headerData[13] = headerData[14] = headerData[15] = 0;

                //cas
                headerData[16] = headerData[17] = headerData[18] = headerData[19] = headerData[20] = headerData[21] = headerData[22] = headerData[23] = 0;
                var @enumerable = new[] { headerData, keyData }.ToArray();
                return @enumerable;
            }

            public static IEnumerable<byte[]> GetRequest(string key)
            {
                var keyData = Encoding.ASCII.GetBytes(key);
                var headerData = new byte[24];
                var totalLength = keyData.Length;
                //magic
                headerData[0] = 0x80;
                //opcode
                headerData[1] = (byte)Command.get;
                //keylength
                headerData[2] = (byte)(keyData.Length >> 8);
                headerData[3] = (byte)(keyData.Length);
                //extralength
                headerData[4] = 0;
                //datatype
                headerData[5] = 0;
                //vbucket
                headerData[6] = 0;
                headerData[7] = 0;

                //totalbody
                headerData[8] = (byte)(totalLength >> 24);
                headerData[9] = (byte)(totalLength >> 16);
                headerData[10] = (byte)(totalLength >> 8);
                headerData[11] = (byte)(totalLength);

                //opaque
                headerData[12] = headerData[13] = headerData[14] = headerData[15] = 0;

                //cas
                headerData[16] = headerData[17] = headerData[18] = headerData[19] = headerData[20] = headerData[21] = headerData[22] = headerData[23] = 0;
                var @enumerable = new[] { headerData, keyData }.ToArray();
                return @enumerable;
            }

            public static IEnumerable<byte[]> InterlockedRequest(Command command, string key, long inter, int expireTime)
            {
                var keyData = Encoding.ASCII.GetBytes(key);
                var extraData = new byte[20];
                var headerData = new byte[24];
                var totalLength = extraData.Length + keyData.Length;

                //delta
                extraData[0] = (byte)(inter >> 56);
                extraData[1] = (byte)(inter >> 48);
                extraData[2] = (byte)(inter >> 40);
                extraData[3] = (byte)(inter >> 32);
                extraData[4] = (byte)(inter >> 24);
                extraData[5] = (byte)(inter >> 16);
                extraData[6] = (byte)(inter >> 8);
                extraData[7] = (byte)inter;

                //initial
                extraData[8] = extraData[9] = extraData[10] = extraData[11] = extraData[12] = extraData[13] = extraData[14] = extraData[15] = 0;

                //expiration
                extraData[16] = (byte)(expireTime >> 24);
                extraData[17] = (byte)(expireTime >> 16);
                extraData[18] = (byte)(expireTime >> 8);
                extraData[19] = (byte)(expireTime);

                //magic
                headerData[0] = 0x80;
                //opcode
                headerData[1] = (byte)command;
                //keylength
                headerData[2] = (byte)(keyData.Length >> 8);
                headerData[3] = (byte)(keyData.Length);
                //extralength
                headerData[4] = (byte)(extraData.Length);
                //datatype
                headerData[5] = 0;
                //vbucket
                headerData[6] = 0;
                headerData[7] = 0;

                //totalbody
                headerData[8] = (byte)(totalLength >> 24);
                headerData[9] = (byte)(totalLength >> 16);
                headerData[10] = (byte)(totalLength >> 8);
                headerData[11] = (byte)(totalLength);

                //opaque
                headerData[12] = headerData[13] = headerData[14] = headerData[15] = 0;

                //cas
                headerData[16] = headerData[17] = headerData[18] = headerData[19] = headerData[20] = headerData[21] = headerData[22] = headerData[23] = 0;
                var @enumerable = new[] { headerData, extraData, keyData }.ToArray();
                return @enumerable;
            }
        }

        #endregion

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

        #endregion
    }
}
