using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using System;
using System.Collections.Generic;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 命令总线
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class RecoveryCommandBus : SerialCommandBus, ICommandBus
    {
        #region field

        /// <summary>
        /// 事件总线
        /// </summary>
        private readonly IEventBus eventBus = null;

        /// <summary>
        /// 领域事件的保存方案
        /// </summary>
        private readonly IEventStorager eventStorager = null;

        /// <summary>
        /// 领域命令储存
        /// </summary>
        private readonly ICommandStorager commandStorager = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryCommandBus"/> class.
        /// </summary>
        /// <param name="serviceLocator">命令提供者</param>
        /// <param name="recoveryStorager">事件存储</param>
        /// <param name="eventStorager">领域事件的保存方案</param>
        /// <param name="commandStorager">命令储存</param>
        public RecoveryCommandBus(IServiceLocator serviceLocator,
            IEventStorager eventStorager,
            ICommandStorager commandStorager,
            IFailRecoveryStorager recoveryStorager)
            : base(serviceLocator)
        {
            this.eventBus = new RecoveryEventBus(this, serviceLocator, recoveryStorager);
            this.eventStorager = eventStorager ?? EmptyEventStreamStorager.Empty;
            this.commandStorager = commandStorager ?? EmptyCommandStreamStorager.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryCommandBus"/> class.
        /// </summary>
        /// <param name="serviceLocator">命令提供者</param>
        /// <param name="eventStreamTypeStorager">领域事件的保存方案</param>
        /// <param name="commandStorager">命令储存</param>
        /// <param name="eventBusCallBack">命令总线初始回调</param>
        protected RecoveryCommandBus(IServiceLocator serviceLocator,
            IEventStorager eventStreamTypeStorager,
            ICommandStorager commandStorager,
            System.Func<RecoveryCommandBus, IServiceLocator, IEventBus> eventBusCallBack)
            : base(serviceLocator)
        {
            this.eventBus = eventBusCallBack(this, serviceLocator);
            this.eventStorager = eventStreamTypeStorager ?? EmptyEventStreamStorager.Empty;
            this.commandStorager = commandStorager ?? EmptyCommandStreamStorager.Empty;
        }

        #endregion ctor

        #region handler events

        /// <summary>
        /// 保存处理命令
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="command">命令</param>
        protected override void HandlerCommandToStorage<TCommand>(ICommandContext commandContext, TCommand command)
        {
            /*保存事件后再执行数据*/
            try
            {
                this.commandStorager.Save(commandContext, command);
            }
            catch
            {
            }
            finally
            {
            }
        }

        /// <summary>
        /// 保存处理事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="eventArray">事件源</param>
        protected override void HandlerEventToStorage(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> eventArray)
        {
            /*保存事件后再执行数据*/
            try
            {
                this.eventStorager.Save(commandContext, eventArray);
            }
            catch
            {
            }
            finally
            {
            }
        }

        /// <summary>
        /// 处理事件，主要是发布事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="eventArray">事件源</param>
        /// <returns></returns>
        protected override void HandlerEventToPublish(ICommandContext commandContext, IEnumerable<IEvent[]> eventArray)
        {
            this.eventBus.Push(commandContext, eventArray);
        }

        #endregion handler events
    }
}