using System;
using System.Threading;

namespace Never.Threading
{
    /// <summary>
    /// 用Interlocked关键字共享，排他锁
    /// </summary>
    public sealed class InterLocker
    {
        #region field and ctor

        /// <summary>
        /// 是否在锁定中
        /// </summary>
        private int locking = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterLocker"/> class.
        /// </summary>
        public InterLocker()
        {
        }

        #endregion field and ctor

        #region lock

        /// <summary>
        /// 以读模式进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        public bool TryEnterLock(Action action)
        {
            if (!this.Enter())
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
                this.Exit();
            }
        }

        /// <summary>
        /// 以读模式进入一个锁
        /// </summary>
        /// <param name="action">回调</param>
        /// <param name="eatException">吃掉异常</param>
        public bool TryEnterLock(Action action, bool eatException)
        {
            if (eatException)
                return this.TryEnterLock(action);

            if (!this.Enter())
                return false;

            try
            {
                action.Invoke();
                return true;
            }
            finally
            {
                this.Exit();
            }
        }

        /// <summary>
        /// 以读模式进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryEnterLock<V>(Func<V> action, out V value)
        {
            value = default(V);
            if (!this.Enter())
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
                this.Exit();
            }
        }

        /// <summary>
        /// 以读模式进入一个锁，并返回值
        /// </summary>
        /// <typeparam name="V">返回值</typeparam>
        /// <param name="action">回调</param>
        /// <param name="value"></param>
        /// <param name="eatException">吃掉异常</param>
        /// <returns></returns>
        public bool TryEnterLock<V>(Func<V> action, bool eatException, out V value)
        {
            if (eatException)
                return this.TryEnterLock(action, out value);

            value = default(V);
            if (!this.Enter())
                return false;

            try
            {
                value = action.Invoke();
                return true;
            }
            finally
            {
                this.Exit();
            }
        }
        /// <summary>
        /// 是否可以锁定
        /// </summary>

        public bool CanLock
        {
            get
            {
                return locking == 1;
            }
        }

        #endregion lock

        #region 线程安全

        /// <summary>
        /// 入锁
        /// </summary>
        private bool Enter()
        {
            if (System.Threading.Interlocked.CompareExchange(ref locking, 0, 1) == 1)
                return true;

            return false;
        }

        /// <summary>
        /// 出锁
        /// </summary>
        private void Exit()
        {
            System.Threading.Interlocked.Exchange(ref locking, 1);
        }

        #endregion 线程安全
    }
}