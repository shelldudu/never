using System;
using System.Collections.Generic;

namespace Never.Collections
{
    /// <summary>
    /// 表示一种先进先出的对象队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IBufferQueue<T> : IDisposable, IEnumerable<T>
    {
        /// <summary>
        /// 当前队列长度
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 队列总长度
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 将某一项队列压入栈
        /// </summary>
        /// <param name="item">数据类型</param>
        bool Enqueue(T item);

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">数据类型</param>
        /// <returns></returns>
        bool Dequeue(out T item);

        /// <summary>
        /// 从队列头中返回某一项，但不删除某项
        /// </summary>
        /// <param name="item">数据类型</param>
        bool Peek(out T item);

        /// <summary>
        /// 队列是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 队列是否已满
        /// </summary>
        bool IsFull { get; }
    }
}