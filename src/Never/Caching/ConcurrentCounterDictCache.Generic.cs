using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Never.Caching
{
    /// <summary>
    /// .net4.0 线性安全的字典 计数缓存，构造参数recyle表示当容量降到该值的时候，会自动回收,该对象实例线程安全
    /// </summary>
    public class ConcurrentCounterDictCache<TKey, TValue> : CounterDictCache<TKey, TValue>
    {
        #region 构造

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentCounterDictCache"/> class.
        /// 构造一个capacity容量，用EqualityComparer对象来比较K的相等性行为
        /// </summary>
        /// <param name="capacity">容量大小</param>
        public ConcurrentCounterDictCache(int capacity)
            : base(capacity, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentCounterDictCache"/> class.
        /// 构造一个capacity容量，用EqualityComparer对象来比较K的相等性行为
        /// </summary>
        /// <param name="capacity">容量大小</param>
        /// <param name="autoRecyleTime">自动回收时间</param>
        public ConcurrentCounterDictCache(int capacity, TimeSpan autoRecyleTime)
            : this(capacity, autoRecyleTime, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentCounterDictCache"/> class.
        /// 构造一个capacity容量，回收缓存对象个数的阀值
        /// </summary>
        /// <param name="comparer">字典中如何对比两个Key对象的接口对象</param>
        /// <param name="capacity">容量大小</param>
        /// <param name="autoRecyleTime">自动回收时间</param>
        public ConcurrentCounterDictCache(int capacity, TimeSpan autoRecyleTime, IEqualityComparer<TKey> comparer)
            : base(capacity, autoRecyleTime, new ConcurrentDictionary<TKey, CacheValueObject>(4 * Environment.ProcessorCount, capacity, comparer), new Never.Threading.ReaderWriterLocker())
        {
        }

        #endregion 构造

        #region 回收算法

        /// <summary>
        /// 回收容量
        /// </summary>
        public override void RecyleCache()
        {
            /*线程启动的时候会执行*/
            if (this.currentCount == 0)
                return;

            this.lastRecyleTime = DateTime.Now;
            /*尝试第一次回收，条件时间过期的对象*/
            var expireList = (from n in this.dict where this.lastRecyleTime.Ticks > n.Value.Ticks select n.Key).ToList();
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
                var hitList = (from n in this.dict select n).OrderByDescending(o => (o.Value.Hit) / ((nowTicks - o.Value.Ticks) == 0 ? o.Value.Hit : (nowTicks - o.Value.Ticks))).ThenBy(o => o.Value.Ticks);
                if (hitList != null)
                {
                    var count = this.recyle < 0 ? 1 : this.recyle;
                    IList<TKey> recyleKey = new List<TKey>(count);
                    /*回收位置从0开始*/
                    int j = 0;
                    for (int i = 0; i < hitList.Count(); i++)
                    {
                        if (j == count)
                            break;
                        recyleKey.Add(hitList.ElementAt(i).Key);
                        j++;
                    }

                    foreach (var key in recyleKey)
                    {
                        this.dict.Remove(key);
                        Interlocked.Decrement(ref this.currentCount);
                    }
                }
            }
        }

        #endregion 回收算法

        #region counter

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <param name="ts">成功回调后插入缓存中过期时间</param>
        /// <returns></returns>
        /// <returns></returns>
        public override TValue GetValue(TKey key, Func<TValue> itemMissCallBack, TimeSpan ts)
        {
            /*在字典中找到了*/
            if (this.dict.TryGetValue(key, out CacheValueObject v) && v != null && v.Ticks >= DateTime.Now.Ticks)
            {
                Interlocked.Increment(ref this.hit);
                return v.AddHit().Value;
            }

            /*miss次数 + 1*/
            Interlocked.Increment(ref this.miss);

            /*是否过期*/
            bool isExpired = v != null && v.Ticks < DateTime.Now.Ticks;
            /*没有回调并且数据过期*/
            if (itemMissCallBack == null && isExpired)
                return default(TValue);

            if (itemMissCallBack == null)
                return default(TValue);

            /*回调方法不为空，则尝试回调该方法*/
            TValue item = itemMissCallBack.Invoke();
            if (item == null)
                return item;

            this.SetValue(key, item, ts);
            return item;
        }

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        public override bool SetValue(TKey key, TValue item, TimeSpan ts)
        {
            if (item == null)
                return false;

            /*回收*/
            if (this.capacity <= this.currentCount)
                this.RecyleCache();

            if (this.capacity > this.currentCount)
            {
                if (this.dict.ContainsKey(key))
                {
                    Interlocked.Decrement(ref this.currentCount);
                    this.dict.Remove(key);
                }

                this.dict[key] = new CacheValueObject(item, DateTime.Now.Add(ts).Ticks);
                /*应该放在下面会更好，不过，字典线性安全，不考虑了*/
                Interlocked.Increment(ref this.currentCount);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 向缓存中插入某一项，有相同的key会出异常
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">缓存的key不能为空</exception>
        public override bool AddValue(TKey key, TValue item, TimeSpan ts)
        {
            if (item == null)
                return false;

            /*回收*/
            if (this.capacity <= this.currentCount)
                this.RecyleCache();

            if (this.capacity > this.currentCount)
            {
                this.dict.Add(key, new CacheValueObject(item, DateTime.Now.Add(ts).Ticks));
                /*应该放在下面会更好，不过，字典线性安全，不考虑了*/
                Interlocked.Increment(ref this.currentCount);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        public override void RemoveValue(TKey key)
        {
            this.dict.Remove(key);
            Interlocked.Decrement(ref this.currentCount);
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemoveValue(TKey key, out TValue value)
        {
            value = default(TValue);
            if (((ConcurrentDictionary<TKey, CacheValueObject>)this.dict).TryRemove(key, out var valueObject))
            {
                Interlocked.Decrement(ref this.currentCount);
                if (valueObject != null && valueObject.Ticks >= DateTime.Now.Ticks)
                {
                    value = valueObject.Value;
                    return true;
                }
            }

            return false;
        }

        #endregion counter
    }
}