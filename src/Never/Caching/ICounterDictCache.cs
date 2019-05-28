namespace Never.Caching
{
    /// <summary>
    /// 计时缓存自动刷新管理
    /// </summary>
    internal interface ICounterDictCache
    {
        /// <summary>
        /// 回收缓存
        /// </summary>
        void RecyleCache();
    }
}