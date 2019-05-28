using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Never.Collections
{
    /// <summary>
    /// 自动扩充的先进先出的单生产者单消费者模式，该方法线性不安全
    /// </summary>
    public sealed class AutoExtendBufferQueue<T> : IBufferQueue<T>, IDisposable
    {
        #region 成员属性

        /// <summary>
        /// 队列最大长度
        /// </summary>
        private int capacity;

        /// <summary>
        /// 队列缓存对象，采取数组方式，避免内存分配碎片过多
        /// </summary>
        private T[] queue;

        /// <summary>
        /// 队列头索引号
        /// </summary>
        private int head;

        /// <summary>
        /// 队列尾部索引号
        /// </summary>
        private int next;

        #endregion 成员属性

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferQueue{T}"/> class.
        /// 构造一个缓冲区容量为Capacity的队列
        /// </summary>
        /// <param name="capacity">缓冲区容量.</param>
        public AutoExtendBufferQueue(int capacity)
        {
            this.capacity = capacity < 2 ? 3 : (capacity + 1);
            this.queue = new T[this.capacity];
            this.head = 0;
            this.next = 0;
        }

        #endregion 构造函数

        #region IBufferQueue成员

        /// <summary>
        /// 返回队列中总长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.capacity - 1;
            }
        }

        /// <summary>
        /// 将某项加入队列
        /// </summary>
        /// <param name="item">数据对象</param>
        /// <returns></returns>
        public bool Enqueue(T item)
        {
            if (this.IsFull)
                this.Extend();

            this.queue[this.next] = item;
            this.next = (this.next + 1) % this.capacity;

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">数据对象</param>
        /// <returns></returns>
        public bool Dequeue(out T item)
        {
            item = default(T);
            if (this.IsEmpty)
                return false;

            this.head = (this.head + 1) % this.capacity;
            item = this.queue[this.head];
            this.queue[this.head] = default(T);

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项，但不删除某项
        /// </summary>
        /// <param name="item">数据对象</param>
        /// <returns></returns>
        public bool Peek(out T item)
        {
            item = default(T);
            if (this.IsEmpty)
                return false;

            item = this.queue[(this.head + 1) % this.capacity];

            return true;
        }

        /// <summary>
        /// 返回当前队列已有数据的长度
        /// </summary>
        public int Count
        {
            get
            {
                if (this.next == this.head)
                    return 0;

                /*尾比头长，表明目前储存的长度还没超capacity个对象，如果不是，则表明尾已循环一次并移动到head前面*/
                if (this.next > this.head)
                    return this.next - this.head;

                return this.capacity + this.next - this.head;
            }
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.head == this.next;
            }
        }

        /// <summary>
        /// 队列是否已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                if (this.head == this.next)
                    return false;

                return (this.next + 1) % this.capacity == this.head;
            }
        }

        #endregion IBufferQueue成员

        #region extend

        /// <summary>
        /// 扩充
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Extend()
        {
            var newCapacity = this.capacity == 3 ? 5 : (this.capacity - 1) * 2;
            var newQueue = new T[newCapacity];
            if (this.head < this.next)
            {
                for (var i = head; i < this.next; i++)
                    newQueue[i] = this.queue[i];
            }
            else
            {
                for (var h = head; h < this.capacity; h++)
                    newQueue[h - head] = this.queue[h];

                for (var h = 0; h < this.next; h++)
                    newQueue[this.capacity - this.head + h] = this.queue[h];

                head = 0;
                next = next + (this.capacity - head);
            }
            this.capacity = newCapacity;
            this.queue = newQueue;
        }

        #endregion extend

        #region IDisposable成员

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.queue != null)
                this.queue = null;
        }

        #endregion IDisposable成员

        #region enumerator

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new BufferQueueEnumerator<T>(this.queue, this.capacity, this.head, this.next);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BufferQueueEnumerator<T>(this.queue, this.capacity, this.head, this.next);
        }

        #endregion enumerator
    }
}