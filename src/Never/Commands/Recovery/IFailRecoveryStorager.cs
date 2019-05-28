using Never.Commands;
using Never.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 执行失败后的存储器
    /// </summary>
    public interface IFailRecoveryStorager
    {
        /// <summary>
        /// 保存一个命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        void Enqueue(IEventContext context, Exception exception, ICommand command);

        /// <summary>
        /// 保存一个命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="communication">通讯上下文</param>
        /// <param name="exception">异常信息</param>
        void Enqueue(HandlerCommunication communication, Exception exception, ICommand command);

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        /// <param name="eventHandlerType">事件处理者</param>
        void Enqueue(IEventContext context, Exception exception, IEvent @event, Type eventHandlerType);

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        /// <param name="eventHandlerType">事件处理者</param>
        void Enqueue(ICommandContext context, Exception exception, IEvent @event, Type eventHandlerType);

        /// <summary>
        /// 返回事件处理
        /// </summary>
        /// <returns></returns>
        RecoveryEventModel DequeueEvent();

        /// <summary>
        /// 返回命令处理
        /// </summary>
        /// <returns></returns>
        RecoveryCommandModel DequeueCommand();
    }
}