using System;

namespace Never.Threading
{
    /// <summary>
    /// 共享锁接口,僵硬的,死板的等待
    /// </summary>
    public interface IRigidLocker : IDisposable
    {
        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        void EnterLock(Action action);

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <returns></returns>
        V EnterLock<V>(Func<V> action);

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        void EnterLock(bool returnNow, Action action);

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        V EnterLock<V>(bool returnNow, Func<V> action);
    }
}