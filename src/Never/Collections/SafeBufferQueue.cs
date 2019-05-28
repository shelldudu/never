using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Never.Collections
{
    /// <summary>
    /// 先进先出的单生产者单消费者模式，该方法线性安全
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public sealed class SafeBufferQueue<T> : IBufferQueue<T>
    {
        #region 成员属性

        /// <summary>
        /// 队列
        /// </summary>
        private IBufferQueue<T> buffer = null;

        /// <summary>
        /// 锁对象
        /// </summary>
        private readonly object lockObject = null;

        #endregion 成员属性

        #region 构造方法

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeBufferStack{T}"/> class.
        /// 构造一个缓冲区容量为Capacity的队列，默认生成BufferQueue类的处理队列
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        public SafeBufferQueue(int capacity) : this(new BufferQueue<T>(capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeBufferStack{T}"/> class.
        /// </summary>
        public SafeBufferQueue(IBufferQueue<T> buffer)
        {
            this.buffer = buffer;
        }

        #endregion 构造方法

        #region IBufferQueue<T>成员

        /// <summary>
        /// 返回当前队列已有数据的长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.buffer.Length;
            }
        }

        /// <summary>
        /// 返回队列中总长度
        /// </summary>
        public int Count
        {
            get
            {
                return this.buffer.Count;
            }
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Length == 0;
            }
        }

        /// <summary>
        /// 队列是否已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                return this.Length == this.Length;
            }
        }

        /// <summary>
        ///  将某项加入队列
        /// </summary>
        /// <param name="item">弹入的对象</param>
        /// <returns></returns>
        public bool Enqueue(T item)
        {
            try
            {
                Monitor.Enter(this.lockObject);
                {
                    return this.buffer.Enqueue(item);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }
        }

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">返回弹出的对象</param>
        /// <returns></returns>
        public bool Dequeue(out T item)
        {
            try
            {
                Monitor.Enter(this.lockObject);
                {
                    return this.buffer.Dequeue(out item);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }
        }

        /// <summary>
        /// 从队列头中返回某一项，但不删除某项
        /// </summary>
        /// <param name="item">返回弹出的对象</param>
        /// <returns></returns>
        public bool Peek(out T item)
        {
            try
            {
                Monitor.Enter(this.lockObject);
                {
                    return this.buffer.Peek(out item);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }
        }

        #endregion IBufferQueue<T>成员

        #region Dispose 成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.buffer.Dispose();
        }

        #endregion Dispose 成员

        #region enumerator

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.buffer.GetEnumerator();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.buffer.GetEnumerator();
        }

        #endregion enumerator
    }
}