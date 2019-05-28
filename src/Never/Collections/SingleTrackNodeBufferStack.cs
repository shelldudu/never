using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Never.Collections
{
    /// <summary>
    /// 后进先出栈模式的链表对象，该对象线程安全
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public sealed class SingleTrackNodeBufferStack<T> : IBufferStack<T>, IDisposable
    {
        #region

        /// <summary>
        /// 当前栈顶位置
        /// </summary>
        private SingleTrackNode<T> top;

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
                return this.top == null;
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
        /// <param name="item">数据</param>
        public bool Push(T item)
        {
            SingleTrackNode<T> current = this.top;
            SingleTrackNode<T> newitem = new SingleTrackNode<T>(item);

            while (true)
            {
                var temp = Interlocked.CompareExchange<SingleTrackNode<T>>(ref this.top, newitem, current);
                if (temp == current)
                {
                    newitem.NextNode = current;
                    Interlocked.Increment(ref this.count);
                    break;
                }
                else
                {
                    current = this.top;
                }
            }

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">数据</param>
        /// <returns></returns>
        public bool Pop(out T item)
        {
            item = default(T);
            if (this.top == null)
                return false;

            SingleTrackNode<T> current = this.top;
            if (this.top == null)
                return false;

            SingleTrackNode<T> newTop = this.top.NextNode;

            while (true)
            {
                var temp = Interlocked.CompareExchange<SingleTrackNode<T>>(ref this.top, newTop, current);
                if (temp == current)
                {
                    item = current.Current;
                    Interlocked.Decrement(ref this.count);
                    break;
                }
                else
                {
                    if (this.top == null)
                        return false;

                    current = this.top;
                    newTop = this.top.NextNode;
                }
            }

            return true;
        }

        /// <summary>
        /// 从队列头中返回某一项，但不移除该项
        /// </summary>
        /// <param name="item">数据</param>
        public bool Peek(out T item)
        {
            item = default(T);
            if (this.top == null)
                return false;

            item = this.top.Current;

            return true;
        }

        #endregion

        #region IDisposable成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.top = null;
        }

        #endregion

        #region enumerator

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var buffer = new T[(this.count)];
            var start = 0;
            var begin = this.top;
            if (begin == null)
                return new BufferStackEnumerator<T>(new T[0], 0);

            var current = begin;
            var next = begin.NextNode;
            do
            {
                var exchange = Interlocked.CompareExchange(ref begin, next, current);
                if (exchange == current)
                {
                    if (exchange == null)
                        break;

                    start++;
                    buffer[buffer.Length - start] = exchange.Current;
                }
                else
                {
                    current = begin;
                    if (begin == null)
                        break;

                    next = begin.NextNode;
                }
            } while (start < buffer.Length);

            return new BufferStackEnumerator<T>(buffer, start);
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