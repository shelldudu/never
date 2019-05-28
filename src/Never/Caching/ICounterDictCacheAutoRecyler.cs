using System;

namespace Never.Caching
{
    /// <summary>
    /// 计时缓存自动刷新管理
    /// </summary>
    internal interface ICounterDictCacheAutoRecyler
    {
        /// <summary>
        /// 注册到自动回收管理处
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="timeSpan"></param>
        void Register(ICounterDictCache cache, TimeSpan timeSpan);
    }
}