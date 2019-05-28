using System;

namespace Never.Threading
{
    /// <summary>
    /// 用Lock关键字共享，排他锁
    /// </summary>
    public sealed class SugerLocker : IRigidLocker, IDisposable
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
        public SugerLocker()
            : this(new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorLocker"/> class.
        /// </summary>
        /// <param name="lockObject">锁对象.</param>
        /// <exception cref="System.ArgumentNullException">lockObject为空</exception>
        public SugerLocker(object lockObject)
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
                lock (this.lockObject)
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
                lock (this.lockObject)
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

        private bool TryEnterLock(TimeSpan timeout, Action action, bool returnNow)
        {
            if (!System.Threading.Monitor.TryEnter(this.lockObject, timeout))
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
                System.Threading.Monitor.Exit(this.lockObject);
                if (returnNow)
                    System.Threading.Interlocked.Exchange(ref locking, 1);
            }
        }

        private bool TryEnterLock<V>(TimeSpan timeout, Func<V> action, bool returnNow, out V value)
        {
            value = default(V);
            if (!System.Threading.Monitor.TryEnter(this.lockObject, timeout))
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
                System.Threading.Monitor.Exit(this.lockObject);
                if (returnNow)
                    System.Threading.Interlocked.Exchange(ref locking, 1);
            }
        }

        #endregion IRigidLocker成员

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