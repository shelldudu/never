using System.Collections;
using System.Collections.Generic;

namespace Never.Collections
{
    internal class BufferStackEnumerator<T> : IEnumerator<T>
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="head"></param>
        public BufferStackEnumerator(T[] queue, int head)
        {
            this.queue = queue;
            this.head = head;
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
        /// 队列索引
        /// </summary>
        private int index;

        #endregion field

        #region IEnumerator

        /// <summary>
        ///
        /// </summary>
        public T Current
        {
            get
            {
                return this.queue[index];
            }
        }

        /// <summary>
        ///
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.queue[index];
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
            /*没有值*/
            if (this.index == 0)
                return false;

            this.index--;
            return true;
        }

        public void Reset()
        {
            this.index = this.head;
        }

        #endregion IEnumerator
    }
}