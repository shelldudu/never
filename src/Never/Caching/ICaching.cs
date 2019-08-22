using System;

namespace Never.Caching
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICaching : IDisposable
    {
        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> itemMissCallBack);

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <param name="ts">成功回调后插入缓存中过期时间</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan ts);

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        void Remove(string key);

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        bool Set<T>(string key, T item);

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        bool Set<T>(string key, T item, TimeSpan ts);

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期，有可能会出现异常
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <returns></returns>
        bool Add<T>(string key, T item);

        /// <summary>
        /// 向缓存中插入某一项，有可能会出现异常
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="item">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        bool Add<T>(string key, T item, TimeSpan ts);
    }
}