#if !NET461
#else

using Never.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.Web.Caching
{
    /// <summary>
    /// .net 4.0 使用内存
    /// </summary>
    public sealed class MemoryCache : ICaching, IDisposable
    {
        #region

        /// <summary>
        /// RuntimeCache 对象
        /// </summary>
        private readonly System.Runtime.Caching.MemoryCache cache = null;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache"/> class.
        /// </summary>
        public MemoryCache()
        {
            this.cache = System.Runtime.Caching.MemoryCache.Default;
        }

        #endregion

        #region ICaching 成员

        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return this.Get<T>(key, null);
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
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            object obj = this.cache.Contains(key) ? this.cache.Get(key) : null;
            if (obj == null)
            {
                if (itemMissCallBack == null)
                    return default(T);

                obj = itemMissCallBack();
                this.Set(key, obj, ts);
            }

            return (T)obj;
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            this.cache.Remove(key);
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
            return this.Set(key, item, TimeSpan.Zero);
        }

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public bool Set<T>(string key, T item, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            if (item == null)
                return false;

            if (ts <= TimeSpan.Zero)
                ts = TimeSpan.FromMinutes(10);
            var policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now + ts
            };
            this.cache.Set(new CacheItem(key, item), policy);
            return true;
        }

        #endregion

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Dispose()
        {
            if (this.cache != null)
                this.cache.Dispose();
        }

        #endregion
    }
}

#endif