using System;
using System.Threading;

namespace Never.Threading
{
    /// <summary>
    /// 用Monitor关键字共享，排他锁
    /// </summary>
    public sealed class MonitorLocker : IRigidLocker, IWaitableLocker, IDisposable
    {
        #region field and ctor

        /// <summary>
        /// 锁对象
        /// </summary>
        private object lockObject = null;

        /// <summary>
        /// 是否在锁定中
        /// </summary>
        private int locking = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorLocker"/> class.
        /// </summary>
        public MonitorLocker()
            : this(new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorLocker"/> class.
        /// </summary>
        /// <param name="lockObject">锁对象.</param>
        /// <exception cref="System.ArgumentNullException">lockObject为空</exception>
        public MonitorLocker(object lockObject)
        {
            if (lockObject == null)
                throw new ArgumentNullException("lockObject为空");

            this.lockObject = lockObject;
        }

        #endregion ctor

        #region IRigidLocker成员

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        public void EnterLock(Action action)
        {
            try
            {
                Monitor.Enter(this.lockObject);
                {
                    action.Invoke();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }
        }

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <returns></returns>
        public V EnterLock<V>(Func<V> action)
        {
            try
            {
                Monitor.Enter(this.lockObject);
                {
                    return action.Invoke();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }
        }

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        public void EnterLock(bool returnNow, Action action)
        {
            if (!returnNow)
            {
                this.EnterLock(action);
                return;
            }

            /*如果当线程获取到资源后，则得到锁*/
            if (System.Threading.Interlocked.CompareExchange(ref locking, 0, 1) == 1)
            {
                //尝试进入锁
                this.TryEnterLock(TimeSpan.FromSeconds(1), action, returnNow);
                return;
            }

            //获取不到锁资源
            return;
        }

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        public V EnterLock<V>(bool returnNow, Func<V> action)
        {
            if (!returnNow)
            {
                return this.EnterLock(action);
            }

            /*如果当线程获取到资源后，则得到锁*/
            if (System.Threading.Interlocked.CompareExchange(ref locking, 0, 1) == 1)
            {
                //尝试进入锁
                if (this.TryEnterLock(TimeSpan.FromSeconds(1), action, returnNow, out var v))
                    return v;
            }

            //获取不到锁资源
            return default(V);
        }

        #endregion IRigidLocker成员

        #region IWaitableLocker成员

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用</param>
        /// <param name="action">回调</param>
        /// <returns></returns>
        public bool TryEnterLock(TimeSpan timeout, Action action)
        {
            return this.TryEnterLock(timeout, action, false);
        }

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用.</param>
        /// <param name="action">回调.</param>
        /// <param name="value">返回值.</param>
        /// <returns></returns>
        public bool TryEnterLock<V>(TimeSpan timeout, Func<V> action, out V value)
        {
            return this.TryEnterLock(timeout, action, false, out value);
        }

        /// <summary>
        /// 进入一个锁
        /// </summary>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用</param>
        /// <param name="action">回调</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        public bool TryEnterLock(bool returnNow, TimeSpan timeout, Action action)
        {
            if (!returnNow)
            {
                this.TryEnterLock(timeout, action);
            }

            /*如果当线程获取到资源后，则得到锁*/
            if (System.Threading.Interlocked.CompareExchange(ref locking, 0, 1) == 1)
            {
                //尝试进入锁
                return this.TryEnterLock(timeout, action, returnNow);
            }

            //获取不到锁资源
            return false;
        }

        /// <summary>
        /// 进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="timeout">过期事件，如果为TimeSpan.Zero则表示该参数不起作用.</param>
        /// <param name="action">回调.</param>
        /// <param name="value">返回值.</param>
        /// <param name="returnNow">如果得不到锁（即锁在工作）是否马上返回：true表示返回，false表示等待</param>
        /// <returns></returns>
        public bool TryEnterLock<V>(bool returnNow, TimeSpan timeout, Func<V> action, out V value)
        {
            if (!returnNow)
            {
                this.TryEnterLock(timeout, action, out value);
            }

            /*如果当线程获取到资源后，则得到锁*/
            if (System.Threading.Interlocked.CompareExchange(ref locking, 0, 1) == 1)
            {
                //尝试进入锁
                return this.TryEnterLock(timeout, action, returnNow, out value);
            }

            //获取不到锁资源
            value = default(V);
            return false;
        }

        private bool TryEnterLock(TimeSpan timeout, Action action, bool returnNow)
        {
            if (!Monitor.TryEnter(this.lockObject, timeout))
                return false;

            try
            {
                action.Invoke();
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
                if (returnNow)
                    System.Threading.Interlocked.Exchange(ref locking, 1);
            }
        }

        private bool TryEnterLock<V>(TimeSpan timeout, Func<V> action, bool returnNow, out V value)
        {
            value = default(V);
            if (!Monitor.TryEnter(this.lockObject, timeout))
                return false;

            try
            {
                value = action.Invoke();
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
                if (returnNow)
                    System.Threading.Interlocked.Exchange(ref locking, 1);
            }
        }

        #endregion IWaitableLocker成员

        #region 释放资源

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
        }

        #endregion 释放资源
    }
}