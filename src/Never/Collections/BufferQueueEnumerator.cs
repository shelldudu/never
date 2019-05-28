using System.Collections;
using System.Collections.Generic;

namespace Never.Collections
{
    internal class BufferQueueEnumerator<T> : IEnumerator<T>
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="capacity"></param>
        /// <param name="head"></param>
        /// <param name="next"></param>
        public BufferQueueEnumerator(T[] queue, int capacity, int head, int next)
        {
            this.queue = queue;
            this.capacity = capacity;
            this.head = head;
            this.next = next;
            this.index = head;
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 队列缓存对象，采取数组方式，避免内存分配碎片过多
        /// </summary>
        private readonly T[] queue;

        /// <summary>
        /// 队列头索引号
        /// </summary>
        private readonly int head;

        /// <summary>
        /// 队列尾部索引号
        /// </summary>
        private readonly int next;

        /// <summary>
        /// 队列索引
        /// </summary>
        private int index;

        /// <summary>
        /// 队列最大长度
        /// </summary>
        private readonly int capacity;

        #endregion field

        #region IEnumerator

        /// <summary>
        ///
        /// </summary>
        public T Current
        {
            get
            {
                return this.queue[index - 1];
            }
        }

        /// <summary>
        ///
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.queue[index - 1];
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            /*同一个位置*/
            if (this.index == this.next)
                return false;

            this.index++;
            if (this.index > this.capacity)
                this.index = 1;
            return true;
        }

        public void Reset()
        {
            this.index = this.head;
        }

        #endregion IEnumerator
    }
}