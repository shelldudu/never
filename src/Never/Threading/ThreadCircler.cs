using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Never.Threading
{
    /// <summary>
    /// 线程循环者
    /// </summary>
    public class ThreadCircler : IDisposable
    {
        #region field and ctor

        /// <summary>
        /// 当前线程
        /// </summary>
        private readonly Thread thread = null;

        /// <summary>
        /// 工作执行者
        /// </summary>
        private Func<TimeSpan> working = null;

        /// <summary>
        /// 异步状态锁
        /// </summary>
        private object syncFlag = null;

        /// <summary>
        /// 是否在执行状态
        /// </summary>
        public bool IsWorking { get; private set; }

        /// <summary>
        /// 是否在等待中
        /// </summary>
        public bool IsWaited { get { return this.flag == -1; } }

        /// <summary>
        ///
        /// </summary>
        private int flag = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="working">返回休息时间，由于一直while，所以尽可能不要返回Timespan.Zero</param>
        /// <param name="threadName"></param>
        public ThreadCircler(Func<TimeSpan> working, string threadName)
        {
            this.working = working;
            this.syncFlag = new object();
            this.thread = new Thread(Working) { IsBackground = true, Name = threadName.IsNullOrEmpty() ? typeof(ThreadCircler).Name : threadName };
        }

        #endregion field and ctor

        #region doing

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="working"></param>
        protected ThreadCircler Replace(Func<TimeSpan> working)
        {
            this.working = working;
            return this;
        }

        private void Working()
        {
            if (working == null)
                return;

            while (true)
            {
                //-1表示外部用来停止了
                if (this.flag == -1)
                {
                    lock (syncFlag)
                    {
                        Monitor.Wait(syncFlag);
                    }
                }

                var timeSpan = TimeSpan.FromSeconds(10);
                try
                {
                    timeSpan = working();
                }
                catch (ThreadAbortException)
                {
                    /*进行了Close操作*/
                }
                catch (ThreadInterruptedException)
                {
                    /*进行了Close操作*/
                }
                catch (Exception ex)
                {
                    this.HandleException(ex, string.Empty);
                }
                finally
                {
                    if (timeSpan != TimeSpan.Zero)
                        Thread.Sleep(timeSpan);
                }
            }
        }

        /// <summary>
        /// 处理日志
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="message">异常消息</param>
        protected virtual void HandleException(Exception ex, string message)
        {
        }

        #endregion doing

        #region state

        /// <summary>
        /// 停止，但不会释放thread
        /// </summary>
        [Obsolete("it means thread wait")]
        public void Stop()
        {
            this.Wait();
        }

        /// <summary>
        /// 停止，但不会释放thread
        /// </summary>
        public void Wait()
        {
            if (!this.IsWorking)
                return;

            System.Threading.Interlocked.Exchange(ref this.flag, -1);
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public void Pulse()
        {
            if (!this.IsWorking)
                return;

            if (this.flag == 0)
                return;

            lock (syncFlag)
            {
                System.Threading.Interlocked.Exchange(ref this.flag, 0);
                Monitor.Pulse(syncFlag);
            }
        }

        /// <summary>
        /// 开始启动
        /// </summary>
        public ThreadCircler Start()
        {
            if (this.IsWorking)
                return this;

            lock (this)
            {
                if (this.IsWorking)
                    return this;

                this.thread.Start();
                this.IsWorking = true;
                this.OnStarting();
            }

            return this;
        }

        /// <summary>
        /// 关闭某一线程
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            this.Close(true);
        }

        /// <summary>
        /// 关闭某一线程
        /// </summary>
        /// <returns></returns>
        private void Close(bool noticed)
        {
            if (!this.IsWorking)
                return;

            try
            {
                if (this.thread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    this.thread.Interrupt();
                }
                else if (!this.thread.Join(10000))
                {
                    this.thread.Abort();
                }

                this.IsWorking = false;

                if (noticed)
                    this.OnClosed();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 在线程启动前执行的动作
        /// </summary>
        protected virtual void OnStarting()
        {
        }

        /// <summary>
        /// 在线程进行释放的时候进行的动作
        /// </summary>
        protected virtual void OnClosed()
        {
        }

        #endregion state

        #region dispose

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
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.Close(false);
        }

        #endregion dispose
    }
}