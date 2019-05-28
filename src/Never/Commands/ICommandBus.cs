using System;
using System.Threading.Tasks;

namespace Never.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">信息</param>
        ICommandHandlerResult Send<TCommand>(TCommand c) where TCommand : ICommand;

        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">信息</param>
        /// <param name="communication">上下文通讯</param>
        ICommandHandlerResult Send<TCommand>(TCommand c, HandlerCommunication communication) where TCommand : ICommand;

        /// <summary>
        /// 异步发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">命令信息</param>
        Task<HandlerCommunication> SendAsync<TCommand>(TCommand c) where TCommand : ICommand;

        /// <summary>
        /// 异步发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">信息</param>
        /// <param name="communication">上下文通讯</param>
        Task<HandlerCommunication> SendAsync<TCommand>(TCommand c, HandlerCommunication communication) where TCommand : ICommand;
    }
}