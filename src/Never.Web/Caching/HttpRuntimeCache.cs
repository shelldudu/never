#if NET461

using Never.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Never.Web.Caching
{
    /// <summary>
    /// 本地缓存，对HttpRuntime.Cache简单的封装
    /// </summary>
    public sealed class HttpRuntimeCache : ICaching, IDisposable
    {
        #region field

        /// <summary>
        /// 缓存对象
        /// </summary>
        private readonly static System.Web.Caching.Cache cache = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="HttpRuntimeCache"/> class.
        /// </summary>
        static HttpRuntimeCache()
        {
            cache = HttpRuntime.Cache;
        }

        #endregion ctor

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

            object obj = cache.Get(key);
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

            cache.Remove(key);
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

            cache.Insert(key, item, null, DateTime.Now.Add(ts), TimeSpan.Zero);
            return true;
        }

        #endregion ICaching 成员

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Dispose()
        {
            if (cache != null)
            {
                var keys = new List<string>(cache.Count);
                var ator = cache.GetEnumerator();
                while (ator.MoveNext())
                    keys.Add(ator.Key as string);

                foreach (var key in keys)
                    cache.Remove(key);
            }
        }

        #endregion IDisposable成员
    }
}

#endif