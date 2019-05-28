using Never.Events;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 事件监听者执行事件前的动作过滤器
    /// </summary>
    public interface IEventActionFilter
    {
        /// <summary>
        /// 动作执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="e">事件</param>
        void OnActionExecuting(IEventContext context, IEvent e);
    }
}