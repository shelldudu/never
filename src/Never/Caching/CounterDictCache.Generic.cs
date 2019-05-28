using Never.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Never.Caching
{
    /// <summary>
    /// 计数缓存，构造参数recyle表示当容量降到该值的时候，会自动回收,该对象实例线程安全
    /// </summary>
    public class CounterDictCache<TKey, TValue> : IDisposable, ICounterDictCache
    {
        #region 缓存对象

        /// <summary>
        /// 缓存对象
        /// </summary>
        protected class CacheValueObject
        {
            /// <summary>
            /// 当前对象值
            /// </summary>
            internal TValue Value;

            /// <summary>
            /// 当前命中
            /// </summary>
            internal long Hit = 0;

            /// <summary>
            /// 对象过期时间包含的刻度数
            /// </summary>
            internal long Ticks;

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheValueObject"/> class.
            /// </summary>
            /// <param name="value">数值</param>
            /// <param name="ticks">过期时间</param>
            public CacheValueObject(TValue value, long ticks)
            {
                this.Value = value;
                this.Hit = 0;
                this.Ticks = ticks;
            }

            /// <summary>
            /// 每命中一次，hit的值会加1
            /// </summary>
            internal CacheValueObject AddHit()
            {
                Interlocked.Increment(ref this.Hit);
                return this;
            }
        }

        /// <summary>
        /// 排序对象
        /// </summary>
        protected struct CacheSortObject : IEquatable<CacheSortObject>, IComparer<CacheSortObject>
        {
            /// <summary>
            ///
            /// </summary>
            public TKey Key { get; private set; }

            /// <summary>
            ///
            /// </summary>
            public long Hits { get; private set; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="key"></param>
            /// <param name="hist"></param>
            public CacheSortObject(TKey key, long hist)
                : this()
            {
                this.Key = key;
                this.Hits = hist;
            }

            /// <summary>
            /// 指示当前对象是否等于同一类型的另一个对象。
            /// </summary>
            /// <param name="other">与此对象进行比较的对象。</param>
            /// <returns>
            /// 如果当前对象等于 <paramref name="other" /> 参数，则为 true；否则为 false。
            /// </returns>
            public bool Equals(CacheSortObject other)
            {
                return this.Hits == other.Hits;
            }

            /// <summary>
            /// 比较
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(CacheSortObject x, CacheSortObject y)
            {
                if (x.Hits == y.Hits)
                    return 0;

                return x.Hits > y.Hits ? 1 : -1;
            }
        }

        #endregion 缓存对象

        #region 属性成员

        /// <summary>
        /// 初始化容量
        /// </summary>
        protected readonly int capacity = 0;

        /// <summary>
        /// 当容量蛮的时候，会回收对象的个数
        /// </summary>
        protected readonly int recyle = 0;

        /// <summary>
        /// 缓存字典
        /// </summary>
        protected readonly IDictionary<TKey, CacheValueObject> dict = null;

        /// <summary>
        /// 读写进行时候锁对象
        /// </summary>
        protected readonly IRigidLocker locker = null;

        /// <summary>
        /// 当前字典中的个数
        /// </summary>
        protected int currentCount = 0;

        /// <summary>
        /// 没有命中的次数
        /// </summary>
        protected int miss = 0;

        /// <summary>
        /// 命中的次数
        /// </summary>
        protected int hit = 0;

        /// <summary>
        /// 上一次回收时间
        /// </summary>
        protected DateTime lastRecyleTime = DateTime.Now;

        #endregion 属性成员

        #region 构造

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterDictCache"/> class.
        /// 构造一个capacity容量，用EqualityComparer对象来比较K的相等性行为
        /// </summary>
        /// <param name="capacity">容量大小</param>
        public CounterDictCache(int capacity)
            : this(capacity, TimeSpan.FromMinutes(10), EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterDictCache"/> class.
        /// 构造一个capacity容量，用EqualityComparer对象来比较K的相等性行为
        /// </summary>
        /// <param name="capacity">容量大小</param>
        /// <param name="autoRecyleTime">自动回收时间</param>
        public CounterDictCache(int capacity, TimeSpan autoRecyleTime)
            : this(capacity, autoRecyleTime, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterDictCache"/> class.
        /// 构造一个capacity容量，回收缓存对象个数的阀值
        /// </summary>
        /// <param name="comparer">字典中如何对比两个Key对象的接口对象</param>
        /// <param name="capacity">容量大小</param>
        public CounterDictCache(int capacity, IEqualityComparer<TKey> comparer)
            : this(capacity, TimeSpan.FromMinutes(10), comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterDictCache"/> class.
        /// 构造一个capacity容量，回收缓存对象个数的阀值
        /// </summary>
        /// <param name="comparer">字典中如何对比两个Key对象的接口对象</param>
        /// <param name="capacity">容量大小</param>
        /// <param name="autoRecyleTime">自动回收时间</param>
        public CounterDictCache(int capacity, TimeSpan autoRecyleTime, IEqualityComparer<TKey> comparer)
            : this(capacity, TimeSpan.FromMinutes(10), new Dictionary<TKey, CacheValueObject>(capacity, comparer), new Never.Threading.MonitorLocker())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterDictCache"/> class.
        /// </summary>
        /// <param name="capacity">容量大小</param>
        /// <param name="dict">字典对象</param>
        /// <param name="autoRecyleTime">自动回收时间</param>
        /// <exception cref="System.ArgumentException">回收容量值不能大于构造容量</exception>
        /// <param name="locker">锁</param>
        protected CounterDictCache(int capacity, TimeSpan autoRecyleTime, IDictionary<TKey, CacheValueObject> dict, IRigidLocker locker)
        {
            this.recyle = capacity % 98;
            this.capacity = capacity;
            this.dict = dict;
            this.locker = locker;
            CounterDictCacheAutoRecyler.Instance.Register(this, autoRecyleTime);
        }

        #endregion 构造

        #region 对外显现内部状态

        /// <summary>
        /// 返回字典容量大小
        /// </summary>
        public int Capacity
        {
            get { return this.capacity; }
        }

        /// <summary>
        /// 回收字典的个数
        /// </summary>
        public int Recyle
        {
            get { return this.recyle; }
        }

        /// <summary>
        /// 返回当前字典中对象的个数
        /// </summary>
        public int CurrentCount
        {
            get { return this.currentCount; }
        }

        /// <summary>
        /// 返回当前没有命中的次数
        /// </summary>
        public int Miss
        {
            get { return this.miss; }
        }

        /// <summary>
        /// 返回当前命中的次数
        /// </summary>
        public int Hit
        {
            get { return this.hit; }
        }

        #endregion 对外显现内部状态

        #region 回收算法

        /// <summary>
        /// 回收容量
        /// </summary>
        public virtual void RecyleCache()
        {
            /*线程启动的时候会执行*/
            if (this.currentCount == 0)
                return;

            this.locker.EnterLock(() =>
            {
                this.lastRecyleTime = DateTime.Now;
                /*尝试第一次回收，条件时间过期的对象*/
                var expireList = new List<TKey>(this.dict.Count);
                foreach (var n in this.dict)
                {
                    if (this.lastRecyleTime.Ticks > n.Value.Ticks)
                        expireList.Add(n.Key);
                }

                if (expireList != null)
                {
                    foreach (var i in expireList)
                    {
                        this.dict.Remove(i);
                        Interlocked.Decrement(ref this.currentCount);
                    }
                }

                /*因以时间过期条件要求回收对象回收后还不够容量，此时会以对象命中次数为条件来回收对象*/
                if (this.capacity <= this.currentCount)
                {
                    /*该算法要改进，同时考虑点击数与过期时间的比率*/
                    var nowTicks = DateTime.Now.Ticks;
                    // List<int>
                    //var hitList = new List<TKey>(this.dict.Count);
                    var sort = new List<CacheSortObject>(this.dict.Count);
                    foreach (var o in this.dict)
                    {
                        sort.Add(new CacheSortObject(o.Key, (o.Value.Hit) / ((nowTicks - o.Value.Ticks) == 0 ? o.Value.Hit : (nowTicks - o.Value.Ticks))));
                    }
                    sort.Sort((x, y) =>
                    {
                        if (x.Equals(y))
                            return 1;

                        return x.Hits < y.Hits ? 1 : -1;
                    });
                    for (var i = 0; i < sort.Count && i < this.recyle; i++)
                    {
                        this.dict.Remove(sort[i].Key);
                        Interlocked.Decrement(ref this.currentCount);
                    }
                }
            });
        }

        #endregion 回收算法

        #region cache

        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public virtual TValue GetValue(TKey key)
        {
            return this.GetValue(key, null);
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中，默认为10分钟过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <returns></returns>
        public virtual TValue GetValue(TKey key, Func<TValue> itemMissCallBack)
        {
            return this.GetValue(key, itemMissCallBack, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <param name="ts">成功回调后插入缓存中过期时间</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual TValue GetValue(TKey key, Func<TValue> itemMissCallBack, TimeSpan ts)
        {
            TValue value = default(TValue);
            CacheValueObject temp = default(CacheValueObject);
            bool isFound = false;
            bool isExpired = false;
            this.locker.EnterLock(() =>
            {
                if (!this.dict.TryGetValue(key, out temp) || temp == null)
                    return;

                if (temp.Ticks <= DateTime.Now.Ticks)
                {
                    isExpired = true;
                    return;
                }

                Interlocked.Increment(ref this.hit);
                isFound = true;
                value = temp.AddHit().Value;
            });

            if (isFound)
                return value;

            /*不成功，原因可能为Key找不到或找到后该缓存对象已过期*/
            /*命中不成功，miss次数增加*/
            Interlocked.Increment(ref this.miss);
            if (isExpired && itemMissCallBack == null)
                return default(TValue);

            if (itemMissCallBack == null)
                return default(TValue);

            value = itemMissCallBack.Invoke();
            this.SetValue(key, value, ts);

            return value;
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public virtual bool SetValue(TKey key, TValue item)
        {
            return this.SetValue(key, item, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual bool SetValue(TKey key, TValue item, TimeSpan ts)
        {
            /*回收*/
            if (this.capacity <= this.currentCount)
            {
                try
                {
                    this.RecyleCache();
                }
                finally
                {
                }
            }

            /*内存爆了，不再加东西了，上面已经回收过一次还是爆*/
            if (this.capacity <= this.currentCount)
                return false;

            return this.locker.EnterLock(() =>
            {
                if (this.dict.ContainsKey(key))
                {
                    this.dict.Remove(key);
                    Interlocked.Decrement(ref this.currentCount);
                }
                this.dict[key] = new CacheValueObject(item, DateTime.Now.Add(ts).Ticks);
                /*应该放在下面会更好，不过，字典线性安全，不考虑了*/
                Interlocked.Increment(ref this.currentCount);

                return true;
            });
        }

        /// <summary>
        /// 向缓存中插新增某一项，默认为10分钟过期，有相同的key会出异常
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        public virtual bool AddValue(TKey key, TValue item)
        {
            return this.AddValue(key, item, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 向缓存中插入某一项，有相同的key会出异常
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual bool AddValue(TKey key, TValue item, TimeSpan ts)
        {
            /*回收*/
            if (this.capacity <= this.currentCount)
            {
                try
                {
                    this.RecyleCache();
                }
                finally
                {
                }
            }

            /*内存爆了，不再加东西了，上面已经回收过一次还是爆*/
            if (this.capacity <= this.currentCount)
                return false;

            return this.locker.EnterLock(() =>
            {
                this.dict.Add(key, new CacheValueObject(item, DateTime.Now.Add(ts).Ticks));
                /*应该放在下面会更好，不过，字典线性安全，不考虑了*/
                Interlocked.Increment(ref this.currentCount);

                return true;
            });
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public virtual void RemoveValue(TKey key)
        {
            this.locker.EnterLock(() =>
            {
                this.dict.Remove(key);
                Interlocked.Decrement(ref this.currentCount);
            });
        }
        #endregion cache

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

            if (this.locker != null)
                this.locker.Dispose();

            if (this.dict.Count == 0)
                return;

            /*不可执行该操作，因为A对象引用了该实例，当A执行资源释放的时候，会调用该实例，但是A如果也是缓存在该实例中，就会进行死循环的调用*/
            //foreach (var x in this.dict)
            //{
            //    var disp = x.Value.Value as IDisposable;
            //    if (disp != null)
            //        disp.Dispose();
            //}

            this.dict.Clear();
        }

        #endregion IDisposable成员

        #region contain

        /// <summary>
        /// 是否包含某个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(TKey key)
        {
            if (!this.dict.ContainsKey(key))
                return false;

            if (!this.dict.TryGetValue(key, out CacheValueObject temp))
                return false;

            if (temp == null || temp.Ticks <= DateTime.Now.Ticks)
                return false;

            return true;
        }

        /// <summary>
        /// 获取某个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[TKey key]
        {
            get
            {
                return this.GetValue(key);
            }
            set
            {
                this.SetValue(key, value);
            }
        }

        #endregion contain
    }
}