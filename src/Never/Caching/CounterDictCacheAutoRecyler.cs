using System;
using System.Collections.Generic;

namespace Never.Caching
{
    /// <summary>
    /// 自动回收缓存管理
    /// </summary>
    internal class CounterDictCacheAutoRecyler : ICounterDictCacheAutoRecyler, IDisposable
    {
        #region field

        /// <summary>
        /// 单例
        /// </summary>
        private readonly static CounterDictCacheAutoRecyler instance = null;

        #endregion field

        #region prop

        /// <summary>
        /// 单例
        /// </summary>
        internal static CounterDictCacheAutoRecyler Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly List<CacheReclyeElement> cacheList = null;

        /// <summary>
        /// 定时器
        /// </summary>
        private readonly System.Threading.Timer timer = null;

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static CounterDictCacheAutoRecyler()
        {
            instance = new CounterDictCacheAutoRecyler();
        }

        /// <summary>
        ///
        /// </summary>
        public CounterDictCacheAutoRecyler()
        {
            this.cacheList = new List<CacheReclyeElement>();
            this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(RepeatWork), null, 1000, 60 * 1000);
        }

        #endregion ctor

        #region 注册cache管理

        /// <summary>
        /// 注册cache管理
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="timeSpan"></param>
        public void Register(ICounterDictCache cache, TimeSpan timeSpan)
        {
            this.cacheList.Add(new CacheReclyeElement()
            {
                Cache = cache,
                RecyleTime = timeSpan,
                LastRecyleTime = DateTime.Now
            });
        }

        #endregion 注册cache管理

        #region 重复工作

        /// <summary>
        /// 重复工作
        /// </summary>
        /// <returns></returns>
        protected void RepeatWork(object @state)
        {
            var temp = new CacheReclyeElement[this.cacheList.Count];
            this.cacheList.CopyTo(temp, 0);

            foreach (var e in temp)
            {
                /*未到回收事件*/
                if (e.LastRecyleTime.Add(e.RecyleTime) < DateTime.Now)
                    continue;

                try
                {
                    e.Cache.RecyleCache();
                    e.LastRecyleTime = DateTime.Now;
                }
                catch
                {
                }
                finally
                {
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposed"></param>
        protected virtual void Dispose(bool disposed)
        {
            if (!disposed)
                return;

            this.timer.Dispose();
        }

        #endregion 重复工作

        #region utils

        /// <summary>
        ///
        /// </summary>
        private class CacheReclyeElement
        {
            public ICounterDictCache Cache { get; set; }
            public TimeSpan RecyleTime { get; set; }
            public DateTime LastRecyleTime { get; set; }
        }

        #endregion utils
    }
}