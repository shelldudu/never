using Never.Commands.Concurrent;
using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.Exceptions;
using Never.IoC;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 简单的串行处理命令总线，只能最大限度保持串行调度管理器，但不能抗拒瞬间并发请求
    /// </summary>
    public class SerialCommandBus : CommandBus, ICommandBus
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialCommandBus"/> class.
        /// </summary>
        /// <param name="serviceLocator">服务定位器</param>
        public SerialCommandBus(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        #endregion ctor

        #region icommandbus

        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="command">信息</param>
        /// <param name="communication">上下文通讯</param>
        public override ICommandHandlerResult Send<TCommand>(TCommand command, HandlerCommunication communication)
        {
            var serialCommand = command as ISerialCommand;
            if (serialCommand == null || string.IsNullOrEmpty(serialCommand.Body))
            {
                return base.Send(command, communication);
            }

            /*获取到权限，则执行任务，在执行完毕后要释放*/
            var row = Database.Use(serialCommand).Select(serialCommand);

            try
            {
                row.WaitOne();
                return base.Send(command, communication);
            }
            catch
            {
                throw;
            }
            finally
            {
                row.Release();
            }
        }

        #endregion icommandbus
    }
}