using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Never.Collections
{
    /// <summary>
    /// 先进先出队列模式的链表对象，该对象线程安全
    /// </summary>
    /// <typeparam name="T">操作的数据类型</typeparam>
    public sealed class SingleTrackNodeBufferQueue<T> : IBufferQueue<T>, IDisposable
    {
        #region

        /// <summary>
        /// 当前队列顶位置
        /// </summary>
        private DoubleTrackNode<T> head;

        /// <summary>
        /// 当前队列尾位置
        /// </summary>
        private DoubleTrackNode<T> end;

        /// <summary>
        /// 队列容量，不确定的数据，只要有数据进，便有数据
        /// </summary>
        private int count = 0;

        #endregion

        #region IBufferStack成员

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.head == null && null == this.end;
            }
        }

        /// <summary>
        /// 队列是否已满，永不true
        /// </summary>
        public bool IsFull
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 返回队列中总长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 返回当前队列已有数据的长度
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 将某一项队列压入栈
        /// </summary>
        /// <param name="item">操作的数据</param>
        public bool Enqueue(T item)
        {
            DoubleTrackNode<T> current = this.end;
            DoubleTrackNode<T> newitem = new DoubleTrackNode<T>(item);

            while (true)
            {
                var temp = Interlocked.CompareExchange<DoubleTrackNode<T>>(ref this.end, newitem, current);
                if (temp == current)
                {
                    /*此时队列没有数据，head==end为头*/
                    if (this.head == null)
                    {
                        this.head = newitem;
                    }

                    if (current != null)
                    {
                        current.NextNode = newitem;
                        newitem.PreNode = current;
                        current = newitem;
                    }

                    Interlocked.Increment(ref this.count);
                    break;
                }
                else
                {
                    current = this.end;
                }
            }

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">操作的数据</param>
        /// <returns></returns>
        public bool Dequeue(out T item)
        {
            item = default(T);
            if (this.head == null)
                return false;

            DoubleTrackNode<T> current = this.head;
            DoubleTrackNode<T> newTop = current == null ? null : current.NextNode;

            while (true)
            {
                var temp = Interlocked.CompareExchange<DoubleTrackNode<T>>(ref this.head, newTop, current);
                if (temp == current)
                {
                    if (current != null)
                    {
                        item = current.Current;
                    }

                    Interlocked.Decrement(ref this.count);
                    break;
                }
                else
                {
                    current = this.head;
                    if (current != null)
                        newTop = current.NextNode;
                }
            }

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项，但不移除该项
        /// </summary>
        /// <param name="item">操作的数据</param>
        public bool Peek(out T item)
        {
            item = default(T);
            if (this.head == null)
                return false;
            item = this.head.Current;
            return true;
        }

        #endregion

        #region IDisposable成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.head = null;
            this.end = null;
        }

        #endregion

        #region enumerator

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var begin = this.head;
            if (begin == null)
                return new BufferQueueEnumerator<T>(new T[0], 0, 0, 0);

            var buffer = new T[(this.count)];
            var start = 0;
            var current = begin;
            var next = begin == null ? null : begin.NextNode;
            do
            {
                var exchange = Interlocked.CompareExchange(ref begin, next, current);
                if (exchange == current)
                {
                    if (exchange == null)
                        break;

                    buffer[start] = exchange.Current;
                    start++;
                }
                else
                {
                    current = begin;
                    if (begin == null)
                        break;

                    next = begin.NextNode;
                }
            } while (start < buffer.Length);

            return new BufferQueueEnumerator<T>(buffer, buffer.Length, 0, start);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion enumerator
    }
}