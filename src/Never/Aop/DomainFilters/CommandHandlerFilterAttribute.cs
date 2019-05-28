using Never.Commands;
using System;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 命令处理者过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandHandlerFilterAttribute : Attribute, ICommandActionFilter
    {
        #region ICommandActionFilter

        /// <summary>
        /// 动作执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="command">命令</param>
        public virtual void OnActionExecuting(ICommandContext context, ICommand command)
        {
#if DEBUG
            Console.WriteLine("command_action_p1");
#endif
        }

        #endregion ICommandActionFilter
    }
}