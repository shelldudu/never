using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Logging;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 命令执行过程中一些元素
    /// </summary>
    public class CommandExcutingElement
    {
        /// <summary>
        /// 日志
        /// </summary>
        public ILoggerBuilder LoggerBuilder { get; set; }

        /// <summary>
        /// 日志属性
        /// </summary>
        public LoggerAttribute LoggerAttribute { get; set; }

        /// <summary>
        /// 命令处理者
        /// </summary>
        public ICommandHandler CommandHandler { get; set; }

        /// <summary>
        /// 命令类型，通常只有一个
        /// </summary>
        public Type CommandHandlerType { get; set; }

        /// <summary>
        /// 命令处理者特性集合
        /// </summary>
        public IEnumerable<CommandHandlerFilterAttribute> HandlerFilters { get; set; }

        /// <summary>
        /// 命令鉴权过滤器
        /// </summary>
        public IEnumerable<CommandHandlerAuthorizeAttribute> AuthorizeFilters { get; set; }

        /// <summary>
        /// 命令上下文
        /// </summary>
        public ICommandContext CommandContext { get; set; }

        /// <summary>
        /// 命令处理者特性集合
        /// </summary>
        public IEnumerable<CommandHandlerFilterAttribute> ExcuteFilters { get; set; }
    }
}