using System;
using System.Collections.Generic;

namespace Never.Collections
{
    /// <summary>
    /// 表示一种后进先出的对象队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IBufferStack<T> : IDisposable, IEnumerable<T>
    {
        /// <summary>
        /// 将某一项队列压入栈
        /// </summary>
        /// <param name="item">加入的对象</param>
        bool Push(T item);

        /// <summary>
        /// 从队列头中返回某一项
        /// </summary>
        /// <param name="item">弹出的对象</param>
        /// <returns></returns>
        bool Pop(out T item);

        /// <summary>
        /// 从队列头中返回某一项，但不移除该项
        /// </summary>
        /// <param name="item">弹出的对象</param>
        bool Peek(out T item);

        /// <summary>
        /// 队列是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 队列是否已满，永不true
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// 返回队列中总长度
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 返回当前队列已有数据的长度
        /// </summary>
        int Count { get; }
    }
}