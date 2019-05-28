using Never.Commands;
using Never.Events;
using System;
using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 事件分析接口
    /// </summary>
    public interface IEventStreamAnalyser
    {
        /// <summary>
        /// 分析事件
        /// </summary>
        /// <param name="context">命令上下文</param>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        IEnumerable<IOperateEvent> Analyse(ICommandContext context, IEnumerable<KeyValuePair<Type, IEvent[]>> events);
    }
}