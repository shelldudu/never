using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Never.Caching
{
    /// <summary>
    ///  .net4.0 线性安全的字典 计数缓存，构造参数recyle表示当容量降到该值的时候，会自动回收,该对象实例线程安全
    /// </summary>
    public class ConcurrentCounterDictCache : ConcurrentCounterDictCache<string, object>, ICaching
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentCounterDictCache"/> class.
        /// 默认10000个缓存对象，在剩下小于10个key后自动回收
        /// </summary>
        public ConcurrentCounterDictCache()
            : this(10000, EqualityComparer<string>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentCounterDictCache"/> class.
        /// </summary>
        /// <param name="capacity">容量大小</param>
        /// <param name="comparer">字典中如何对比两个Key对象的接口对象</param>
        public ConcurrentCounterDictCache(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, TimeSpan.FromMinutes(10), comparer)
        {
        }

        #endregion ctor

        #region ICaching成员

        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public virtual T Get<T>(string key)
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
        public virtual T Get<T>(string key, Func<T> itemMissCallBack)
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
        public virtual T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan ts)
        {
            if (itemMissCallBack == null)
                return (T)base.GetValue(key, null);

            return (T)base.GetValue(key, new Func<object>(() => { return itemMissCallBack.Invoke(); }), ts);
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual void Remove(string key)
        {
            base.RemoveValue(key);
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public virtual bool Set<T>(string key, T item)
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
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual bool Set<T>(string key, T item, TimeSpan ts)
        {
            return base.SetValue(key, item, ts);
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期，有可能会出现异常
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public virtual bool Add<T>(string key, T item)
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
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual bool Add<T>(string key, T item, TimeSpan ts)
        {
            return base.AddValue(key, item, ts);
        }
        #endregion ICaching成员

        #region dispose

        /// <summary>
        /// 释放连接对象
        /// </summary>
        /// <param name="isdispose">是否释放</param>
        protected override void Dispose(bool isdispose)
        {
            base.Dispose(isdispose);
        }

        #endregion dispose
    }
}