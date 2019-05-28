using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Logging;
using System;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// 事件执行过程中一些元素
    /// </summary>
    public struct EventExcutingElement
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public ILoggerBuilder LoggerBuilder { get; set; }

        /// <summary>
        /// 日志属性
        /// </summary>
        public LoggerAttribute LoggerAttribute { get; set; }

        /// <summary>
        /// 事件处理者
        /// </summary>
        public IEventHandler EventHandler { get; set; }

        /// <summary>
        /// 事件类型，通常只有一个
        /// </summary>
        public Type EventHandlerType { get; set; }

        /// <summary>
        /// 事件处理者特性集合
        /// </summary>
        public IEnumerable<EventHandlerFilterAttribute> HandlerFilters { get; set; }

        /// <summary>
        /// 事件上下文
        /// </summary>
        public IEventContext EventContext { get; set; }
    }
}