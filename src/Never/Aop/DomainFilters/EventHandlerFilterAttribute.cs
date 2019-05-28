using Never.Events;
using System;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EventHandlerFilterAttribute : Attribute, IEventActionFilter
    {
        #region IEventActionFilter

        /// <summary>
        /// 动作执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="e">事件</param>
        public virtual void OnActionExecuting(Events.IEventContext context, IEvent e)
        {
#if DEBUG
            Console.WriteLine("event_action");
#endif
        }

        #endregion IEventActionFilter
    }
}