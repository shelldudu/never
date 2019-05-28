using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Exceptions;
using Never.IoC;
using Never.Logging;
using System;

namespace Never.Commands
{
    /// <summary>
    /// 命令扩展
    /// </summary>
    public static class CommandBusExcutingHelper
    {
        #region element
        /// <summary>
        /// 找出当前的命令信息
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="provider"></param>
        /// <param name="command">信息</param>
        /// <param name="communication">上下文通讯</param>
        /// <returns></returns>
        internal static CommandExcutingElement FindCommandExcutingElement<TCommand>(CommandHandlerProvider provider, TCommand command, HandlerCommunication communication) where TCommand : ICommand
        {
            /*查找事件监听者*/
            var commandListeners = provider.FindCommandHandler<TCommand>();
            if (commandListeners == null || commandListeners.Length <= 0)
                return new CommandExcutingElement();

            if (commandListeners.Length >= 2)
                throw new InvalidException(string.Format("the command {0} has more handler", typeof(TCommand).Name));

            var helper = new CommandExcutingElement();
            /*命令上下文*/
            var commandContext = provider.FindCommandContext();
            var defaultContext = commandContext as ICommandContextInitable;
            if (defaultContext != null)
                defaultContext.OnInit(communication, command);

            helper.CommandContext = commandContext;

            /*发布命令*/
            var handler = commandListeners[0];
            var handlerType = handler.GetType();
            helper.CommandHandler = commandListeners[0];
            helper.CommandHandlerType = handler.GetType();

            /*handler所有属性*/
            var handlerAttributes = HandlerBehaviorStorager.Default.GetAttributes(handlerType);
            var excuteAttributes = HandlerBehaviorStorager.Default.GetAttributes(handlerType, command.GetType());
            var handlerFilters = ObjectExtension.GetAttributes<CommandHandlerFilterAttribute>(handlerAttributes) ?? new CommandHandlerFilterAttribute[] { };
            var excuteFilters = ObjectExtension.GetAttributes<CommandHandlerFilterAttribute>(excuteAttributes) ?? new CommandHandlerFilterAttribute[] { };
            helper.HandlerFilters = handlerFilters;
            helper.ExcuteFilters = excuteFilters;

            /*全局命令过滤器*/
            var authorizeAttributes = ObjectExtension.GetAttributes<CommandHandlerAuthorizeAttribute>(handlerAttributes) ?? new CommandHandlerAuthorizeAttribute[] { };
            helper.AuthorizeFilters = authorizeAttributes;

            /*日志预留接口*/
            var loggerAttribute = ObjectExtension.GetAttribute<LoggerAttribute>(handlerAttributes);
            if (loggerAttribute != null)
            {
                helper.LoggerAttribute = loggerAttribute;
                try
                {
                    var loggerBuilder = loggerAttribute.RegisterKey.IsNotNullOrEmpty() ? provider.Scope.Resolve<ILoggerBuilder>(loggerAttribute.RegisterKey) : provider.Scope.ResolveOptional<ILoggerBuilder>();
                    helper.LoggerBuilder = loggerBuilder ?? LoggerBuilder.Empty;
                }
                catch
                {
                }
            }

            return helper;
        }
        #endregion

        #region runtime
        /// <summary>
        /// 新加运行时类型到上下文
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="communication">上下文通讯</param>
        /// <param name="runtimeMode">当前运行模式</param>
        public static void AddRuntimeMode(this ICommandBus commandBus, HandlerCommunication communication, string runtimeMode)
        {
            if (communication == null)
                return;

            AddRuntimeMode(commandBus, communication, new Aop.DefaultRuntimeMode() { RuntimeMode = runtimeMode });
        }

        /// <summary>
        /// 新加运行时类型到上下文
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="communication">上下文通讯</param>
        /// <param name="runtimeMode">当前运行模式</param>
        public static void AddRuntimeMode(this ICommandBus commandBus, HandlerCommunication communication, Aop.IRuntimeMode runtimeMode)
        {
            if (communication == null || runtimeMode == null)
                return;

            communication.RuntimeModeArray.Add(runtimeMode);
        }
        #endregion
    }
}