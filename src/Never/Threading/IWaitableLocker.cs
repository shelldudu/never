using System;

namespace Never.Threading
{
    /// <summary>
    /// 尝试共享锁,可超时的等待
    /// </summary>
    public interface IWaitableLocker : IDisposable
    {
        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用</param>
        /// <param name="action">回调</param>
        /// <returns></returns>
        bool TryEnterLock(TimeSpan timeout, Action action);

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用.</param>
        /// <param name="action">回调.</param>
        /// <param name="value">返回值.</param>
        /// <returns></returns>
        bool TryEnterLock<V>(TimeSpan timeout, Func<V> action, out V value);

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用</param>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        bool TryEnterLock(bool returnNow, TimeSpan timeout, Action action);


        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用.</param>
        /// <param name="action">回调.</param>
        /// <param name="value">返回值.</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        bool TryEnterLock<V>(bool returnNow, TimeSpan timeout, Func<V> action, out V value);
    }
}