using System;
using System.Collections.Generic;

namespace Never.Commands.Concurrent
{
    /// <summary>
    /// 将命令当成Table，即每一个并发的命令对应一个Table，Table里面有许多行，每一行就是并发命令的主键
    /// </summary>
    internal class Table
    {
        #region dict

        /// <summary>
        /// 存储每一个命令的表行记录对象
        /// </summary>
        private readonly SortedDictionary<string, TableRow> sorted = new SortedDictionary<string, TableRow>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 锁对象
        /// </summary>
        private readonly object lockObject = null;

        #endregion dict

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public Table()
        {
            this.lockObject = new object();
        }

        #endregion ctor

        /// <summary>
        /// 查询某一行TableRow对象
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public TableRow Select(ISerialCommand command)
        {
            /*核心点是如何在多线程下只会创建一条记录*/
            if (this.sorted.ContainsKey(command.Body))
                return sorted[command.Body];

            /*创建一行记录太快，这里的锁的性能可以忽略*/
            System.Threading.Monitor.Enter(lockObject);
            try
            {
                if (this.sorted.ContainsKey(command.Body))
                    return sorted[command.Body];

                return sorted[command.Body] = new TableRow();
            }
            catch
            {
            }
            finally
            {
                System.Threading.Monitor.Exit(lockObject);
            }

            return sorted[command.Body] = new TableRow();
        }

        /// <summary>
        /// 获取命令个数
        /// </summary>
        public int CommandCount
        {
            get
            {
                int count = 0;
                foreach (var i in sorted)
                {
                    count += i.Value.CommandCount;
                }

                return count;
            }
        }
    }
}