using Never.Commands;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 事件监听者执行事件前的动作过滤器
    /// </summary>
    public interface ICommandActionFilter
    {
        /// <summary>
        /// 动作执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="command">命令</param>
        void OnActionExecuting(ICommandContext context, ICommand command);
    }
}