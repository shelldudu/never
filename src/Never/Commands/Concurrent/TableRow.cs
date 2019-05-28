using System.Collections.Generic;

namespace Never.Commands.Concurrent
{
    /// <summary>
    /// Table中的一行记录，即穿行命令的主键所定位到的对象
    /// </summary>
    internal class TableRow
    {
        #region prop

        /// <summary>
        /// 生产者mailbox记录冲突的命令
        /// </summary>
        private System.Collections.Concurrent.ConcurrentQueue<ICommand> producerMailBox = null;

        /// <summary>
        /// 消费者mailbox记录冲突的命令
        /// </summary>
        private System.Collections.Concurrent.ConcurrentQueue<ICommand> consumerMailBox = null;

        /// <summary>
        /// 使用同步过程中锁对象
        /// </summary>
        private readonly object lockObject = null;

        /// <summary>
        /// 是否在锁定中
        /// </summary>
        private int locking = 1;

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public TableRow()
        {
            this.lockObject = new object();
            this.producerMailBox = new System.Collections.Concurrent.ConcurrentQueue<ICommand>();
            this.consumerMailBox = new System.Collections.Concurrent.ConcurrentQueue<ICommand>();
        }

        #endregion ctor

        #region dequeue

        /// <summary>
        /// 弹出一个命令
        /// </summary>
        /// <returns></returns>
        public ICommand Dequeue()
        {
            /*有数据，马上弹出一个命令*/
            var command = default(ICommand);
            if (consumerMailBox.TryDequeue(out command))
                return command;

            /*进行交换引用*/
            var temp = producerMailBox;
            producerMailBox = consumerMailBox;
            consumerMailBox = temp;

            consumerMailBox.TryDequeue(out command);
            return command;
        }

        /// <summary>
        /// 压入一个命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommand Enqueue(ICommand command)
        {
            this.producerMailBox.Enqueue(command);
            return command;
        }

        #endregion dequeue

        #region count

        /// <summary>
        /// 获取当前队列中命令的个数
        /// </summary>
        public int CommandCount
        {
            get
            {
                return this.consumerMailBox.Count + this.producerMailBox.Count;
            }
        }

        #endregion count

        #region waitone

        /// <summary>
        /// 将当前状态更新为未锁定状态
        /// </summary>
        public void Reset()
        {
            System.Threading.Interlocked.Exchange(ref locking, 1);
        }

        /// <summary>
        /// 等待资源，直至线程变得可用，该方法通常用于同步发送命令，如果是异步，则可以使用队列方式
        /// </summary>
        public void WaitOne()
        {
            System.Threading.Monitor.Enter(lockObject);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            System.Threading.Monitor.Exit(lockObject);
        }

        #endregion waitone
    }
}