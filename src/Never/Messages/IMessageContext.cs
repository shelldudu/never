using System;
using System.Collections.Generic;

namespace Never.Messages
{
    /// <summary>
    /// 消息上下文
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// 上下文集合
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// 当前执行的对象类型
        /// </summary>
        Type TargetType { get; }
    }
}