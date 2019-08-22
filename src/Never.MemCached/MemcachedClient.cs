using System;
using System.Linq;
using System.Text;
using Never.Caching;

namespace Never.Memcached
{
    /// <summary>
    /// memcached client
    /// </summary>
    public abstract class MemcachedClient : ICaching
    {
        #region field and ctor

        private readonly ICompressProtocol compress = null;

        protected MemcachedClient(ICompressProtocol compress)
        {
            this.compress = compress;
        }

        #endregion field and ctor

        #region opear

        /// <summary>
        /// 尝试加一个key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAddValue(string key, object value)
        {
            return this.TryAddValue(key, value, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryAddValue(string key, object value, TimeSpan expireTime)
        {
            if (value == null)
                return false;

            var @byte = this.compress.Compress(value, out var flag);
            return this.TryAddValue(key, @byte, flag, expireTime);
        }

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryAddResult<T>(string key, T value, TimeSpan expireTime)
        {
            var @byte = this.compress.Compress(value, out var flag);
            return this.TryAddValue(key, @byte, flag, expireTime);
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
        protected abstract bool TryAddValue(string key, byte[] value, byte flag, TimeSpan expireTime);

        /// <summary>
        /// 尝试加一个key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetValue(string key, object value)
        {
            return this.TrySetValue(key, value, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TrySetValue(string key, object value, TimeSpan expireTime)
        {
            if (value == null)
                return false;

            var @byte = this.compress.Compress(value, out var flag);
            return this.TrySetValue(key, @byte, flag, expireTime);
        }

        /// <summary>
        /// 尝试加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TrySetResult<T>(string key, T value, TimeSpan expireTime)
        {
            var @byte = this.compress.Compress(value, out var flag);
            return this.TrySetValue(key, @byte, flag, expireTime);
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
        protected abstract bool TrySetValue(string key, byte[] value, byte flag, TimeSpan expireTime);

        /// <summary>
        /// 尝试替换一个key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryReplaceValue(string key, object value)
        {
            return this.TryReplaceValue(key, value, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 尝试替换一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryReplaceValue(string key, object value, TimeSpan expireTime)
        {
            if (value == null)
                return false;

            var @byte = this.compress.Compress(value, out var flag);
            return this.TryReplaceValue(key, @byte, flag, expireTime);
        }

        /// <summary>
        /// 尝试替换一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryReplaceResult<T>(string key, T value, TimeSpan expireTime)
        {
            var @byte = this.compress.Compress(value, out var flag);
            return this.TryReplaceValue(key, @byte, flag, expireTime);
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
        protected abstract bool TryReplaceValue(string key, byte[] value, byte flag, TimeSpan expireTime);

        /// <summary>
        /// 尝试在此之后新加一个key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAppendValue(string key, object value)
        {
            return this.TryAppendValue(key, value, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 尝试在此之后新加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryAppendValue(string key, object value, TimeSpan expireTime)
        {
            if (value == null)
                return false;

            var @byte = this.compress.Compress(value, out var flag);
            return this.TryAppendValue(key, @byte, flag, expireTime);
        }

        /// <summary>
        /// 尝试在此之后新加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryAppendResult<T>(string key, T value, TimeSpan expireTime)
        {
            var @byte = this.compress.Compress(value, out var flag);
            return this.TryAppendValue(key, @byte, flag, expireTime);
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
        protected abstract bool TryAppendValue(string key, byte[] value, byte flag, TimeSpan expireTime);

        /// <summary>
        /// 尝试在此之前新加一个key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryPrependValue(string key, object value)
        {
            return this.TryPrependValue(key, value, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 尝试在此之前新加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryPrependValue(string key, object value, TimeSpan expireTime)
        {
            if (value == null)
                return false;

            var @byte = this.compress.Compress(value, out var flag);
            return this.TryPrependValue(key, @byte, flag, expireTime);
        }

        /// <summary>
        /// 尝试在此之前新加一个key并指定过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public virtual bool TryPrependResult<T>(string key, T value, TimeSpan expireTime)
        {
            var @byte = this.compress.Compress(value, out var flag);
            return this.TryPrependValue(key, @byte, flag, expireTime);
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
        protected abstract bool TryPrependValue(string key, byte[] value, byte flag, TimeSpan expireTime);

        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(string key, out object value);

        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGetResult<T>(string key, out T value)
        {
            if (this.TryGetValue(key, out var @object))
            {
                value = (T)@object;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool TryDelete(string key);

        /// <summary>
        /// 对值-1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="decrement"></param>
        /// <returns></returns>
        public virtual bool TryDecrement(string key, long decrement)
        {
            return this.TryInterlocked(Command.decrement, key, decrement, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 对值-1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="decrement"></param>
        /// <returns></returns>
        public virtual bool TryDecrement(string key, long decrement, TimeSpan expireTime)
        {
            return this.TryInterlocked(Command.decrement, key, decrement, expireTime);
        }

        /// <summary>
        /// 对值+1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public virtual bool TryIncrement(string key, long increment)
        {
            return this.TryInterlocked(Command.increment, key, increment, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 对值+1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public virtual bool TryIncrement(string key, long increment, TimeSpan expireTime)
        {
            return this.TryInterlocked(Command.increment, key, increment, expireTime);
        }

        /// <summary>
        /// 对值+-1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inter"></param>
        /// <returns></returns>
        protected abstract bool TryInterlocked(Command command, string key, long inter, TimeSpan expireTime);

        #endregion opear

        #region help

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int SumASCII(string routeKey)
        {
            var index = 0;
            if (string.IsNullOrWhiteSpace(routeKey))
                return index;

            foreach (var @char in routeKey.ToArray())
            {
                index += @char;
            }

            return index;
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int SumASCII(long routeKey)
        {
            return this.SumASCII(Math.Abs(routeKey).ToString());
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="uniqueId">路由Key</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int SumASCII(Guid uniqueId)
        {
            return this.SumASCII(uniqueId.ToString());
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int SumASCII<TKey>(TKey routeKey)
        {
            return this.SumASCII(routeKey.ToString());
        }

        /// <summary>
        /// 是否全部为数字
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IsDigit(string content)
        {
            if (content == null)
                return false;

            for (var i = 0; i < content.Length; i++)
            {
                if (content[i] < '0' || content[i] > '9')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 按一定顺序返回对应的数组索引
        /// </summary>
        /// <param name="hv"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int[] SortServer(int hv, int length)
        {
            if (length == 1)
                return new int[] { 0 };

            for (var i = 0; i < length; i++)
            {
                var min = i * (int.MaxValue / length);
                var max = (i + 1) * (int.MaxValue / length);
                if (hv.IsBetween(min, max))
                    return new[] { i };
            }

            return new[] { hv % length };
            //var array = new int[length];
            //var temp = new int[length];
            //for (var i = 0; i < length; i++)
            //    temp[i] = i;

            //var hit = hv % length;
            //temp[hit] = -1;
            //array[0] = hit;
            //var start = 0;
            //var offset = 0;
            //do
            //{
            //    length--;
            //    start = 0;
            //    offset = 0;
            //    hit = ++hv % length;
            //    for (start = 0; start < temp.Length; start++)
            //    {
            //        if (temp[start] == -1)
            //        {
            //            offset++;
            //            continue;
            //        }

            //        if (start != hit + offset)
            //            continue;

            //        array[length] = temp[start];
            //        temp[start] = -1;
            //        break;
            //    }
            //} while (length > 1);

            //return array;
        }

        /// <summary>
        /// 返回总的秒数
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        protected int GetTotalSecond(TimeSpan timeSpan)
        {
            /*最长一个月*/
            if (timeSpan.TotalSeconds > 30 * 24 * 60 * 60)
                return 30 * 24 * 60 * 60;

            var second = (int)timeSpan.TotalSeconds;
            return second <= 0 ? 1 : second;
        }

        #endregion help

        #region create

        /// <summary>
        /// 地址转换
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static System.Net.IPEndPoint Parse(string service)
        {
            var port = string.Empty;
            var host = System.Text.RegularExpressions.Regex.Replace(service, ":(?<name>\\d+)", (m) =>
            {
                port = m.Groups["name"].Value;
                return string.Empty;
            });

            return new System.Net.IPEndPoint(System.Net.IPAddress.Parse(host), port.AsInt());
        }

        /// <summary>
        /// 创建文件协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public static MemcachedClient CreateTextCached(string[] servers, ICompressProtocol compress)
        {
            return CreateTextCached(servers, Encoding.UTF8, compress);
        }

        /// <summary>
        /// 创建文件协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static MemcachedClient CreateTextCached(string[] servers, Encoding encoding, ICompressProtocol compress)
        {
            return CreateTextCached(servers, encoding, compress, new SocketSetting());
        }

        /// <summary>
        /// 创建文件协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="encoding"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static MemcachedClient CreateTextCached(string[] servers, Encoding encoding, ICompressProtocol compress, SocketSetting setting)
        {
            return new TextCached(servers.Select(ta => Parse(ta)).ToArray(), encoding, compress, setting ?? new SocketSetting());
        }

        /// <summary>
        /// 创建二进制协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public static MemcachedClient CreateBinaryCached(string[] servers, ICompressProtocol compress)
        {
            return CreateBinaryCached(servers, Encoding.UTF8, compress);
        }

        /// <summary>
        /// 创建二进制协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static MemcachedClient CreateBinaryCached(string[] servers, Encoding encoding, ICompressProtocol compress)
        {
            return CreateBinaryCached(servers, encoding, compress, new SocketSetting());
        }

        /// <summary>
        /// 创建二进制协议的client
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="encoding"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static MemcachedClient CreateBinaryCached(string[] servers, Encoding encoding, ICompressProtocol compress, SocketSetting setting)
        {
            return new BinaryCached(servers.Select(ta => Parse(ta)).ToArray(), encoding, compress, setting ?? new SocketSetting());
        }

        #endregion create

        #region icaching

        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (this.TryGetResult<T>(key, out T value))
                return value;

            return default(T);
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> itemMissCallBack)
        {
            return this.Get<T>(key, itemMissCallBack, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <param name="ts">成功回调后插入缓存中过期时间</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan expireTime)
        {
            if (this.TryGetResult<T>(key, out T value))
                return value;

            if (itemMissCallBack == null)
                return default(T);

            value = itemMissCallBack();
            this.TrySetResult(key, value, expireTime);
            return value;
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        public void Remove(string key)
        {
            this.TryDelete(key);
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public bool Set<T>(string key, T item)
        {
            return this.Set<T>(key, item, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T item, TimeSpan expireTime)
        {
            return this.TrySetResult(key, item, expireTime);
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期，有可能会出现异常
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public bool Add<T>(string key, T item)
        {
            return this.Add<T>(key, item, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 向缓存中插入某一项，有可能会出现异常
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        public bool Add<T>(string key, T item, TimeSpan expireTime)
        {
            return this.TryAddResult(key, item, expireTime);
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
        /// <param name="dispose"></param>
        protected virtual void Dispose(bool dispose)
        {
            this.compress.Dispose();
        }

        #endregion icaching
    }
}