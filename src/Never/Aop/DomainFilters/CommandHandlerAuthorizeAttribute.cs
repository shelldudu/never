using Never.Commands;
using System;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 命令处理者授权验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandHandlerAuthorizeAttribute : Attribute
    {
        /// <summary>
        /// 验证命令是否通过
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public virtual bool Validate(ICommandContext context, ICommand command)
        {
            return true;
        }
    }
}