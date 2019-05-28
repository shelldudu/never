using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Never.Collections
{
    /// <summary>
    /// 自动扩充的单生产者单消费者模式(栈)，该方法线性不安全
    /// </summary>
    public sealed class AutoExtendBufferStatck<T> : IBufferStack<T>, IDisposable
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
        /// 队列索引号
        /// </summary>
        private int head;

        #endregion 成员属性

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferStack{T}"/> class.
        /// 构造一个缓冲区容量为Capacity的队列
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        public AutoExtendBufferStatck(int capacity)
        {
            this.queue = new T[capacity];
            this.capacity = capacity;
            this.head = 0;
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
                return this.capacity;
            }
        }

        /// <summary>
        ///  将某项加入队列
        /// </summary>
        /// <param name="item">加入的对象</param>
        /// <returns></returns>
        public bool Push(T item)
        {
            if (this.IsFull)
                this.Extend();

            this.queue[this.head] = item;
            this.head = (this.head + 1);

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">弹出的对象</param>
        /// <returns></returns>
        public bool Pop(out T item)
        {
            item = default(T);
            if (this.IsEmpty)
                return false;

            this.head = this.head - 1;
            item = this.queue[this.head];
            this.queue[this.head] = default(T);

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">弹出的对象</param>
        /// <returns></returns>
        public bool Peek(out T item)
        {
            item = default(T);
            if (this.IsEmpty)
                return false;

            item = this.queue[this.head - 1];

            return true;
        }

        /// <summary>
        /// 返回当前队列已有数据的长度
        /// </summary>
        public int Count
        {
            get
            {
                return this.head;
            }
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.head == 0;
            }
        }

        /// <summary>
        /// 队列是否已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                return this.head == this.capacity;
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
            for (var i = 0; i < this.head; i++)
                newQueue[i] = this.queue[i];

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
            return new BufferStackEnumerator<T>(this.queue, this.head);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BufferStackEnumerator<T>(this.queue, this.head);
        }

        #endregion enumerator
    }
}