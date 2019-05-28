using System;
using System.Collections;

namespace Never.Caching
{
    /// <summary>
    /// 上下文缓存，通常都是短暂或者线程的
    /// </summary>
    public abstract class ContextCache : ICaching, IDisposable, IEnumerable
    {
        #region field

        /// <summary>
        /// 缓存对象
        /// </summary>
        private readonly IDictionary table = null;

        #endregion field

        #region cacheobject

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class CacheObject<T>
        {
            public T Instance { get; set; }
        }

        #endregion cacheobject

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextCache"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        protected ContextCache(IDictionary cache)
        {
            this.table = cache;
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
        public virtual T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            var obj = this.table[key] as CacheObject<T>;
            if (obj == null)
            {
                if (itemMissCallBack == null)
                    return default(T);

                var item = itemMissCallBack();
                this.Set(key, item, ts);
                return item;
            }

            return obj.Instance;
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            this.table.Remove(key);
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
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual bool Set<T>(string key, T item, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            var obj = new CacheObject<T>() { Instance = item };
            this.table[key] = obj;
            return true;
        }

        #endregion ICaching 成员

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放内部资源
        /// </summary>
        /// <param name="isDispose">是否释放</param>
        protected virtual void Dispose(bool isDispose)
        {
            if (!isDispose)
                return;

            if (this.table != null)
                this.table.Clear();
        }

        #endregion IDisposable成员

        #region clear

        /// <summary>
        /// 对象中移除所有元素
        /// </summary>
        public void Clear()
        {
            this.table.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)table).GetEnumerator();
        }

        #endregion clear
    }
}