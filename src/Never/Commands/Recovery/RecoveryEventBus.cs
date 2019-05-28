using Never.Events;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 发布事件
    /// </summary>
    internal sealed class RecoveryEventBus : EventBus, IEventBus
    {
        private readonly EventRecoveryManager eventRecovery = null;
        private readonly CommandRecoveryManager commandRecovery = null;
        private readonly IFailRecoveryStorager recoveryStorager = null;
        internal readonly RecoveryCommandBus commandBus = null;
        internal readonly IServiceLocator serviceLocator = null;

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryEventBus"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        /// <param name="recoveryStorager">The recovery storage.</param>
        public RecoveryEventBus(RecoveryCommandBus commandBus,
            IServiceLocator serviceLocator,
            IFailRecoveryStorager recoveryStorager) : base(commandBus, serviceLocator)
        {
            this.commandBus = commandBus;
            this.serviceLocator = serviceLocator;
            this.recoveryStorager = recoveryStorager;
            this.commandRecovery = new CommandRecoveryManager(recoveryStorager, serviceLocator, commandBus, this);
            this.eventRecovery = new EventRecoveryManager(recoveryStorager, serviceLocator, commandBus, this);
            this.commandRecovery.Start();
            this.eventRecovery.Start();
        }

        #endregion ctor

        #region push

        public override void Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
            if (events == null)
                return;

            foreach (var splits in events)
            {
                if (splits.IsNullOrEmpty())
                    continue;

                var task = Task.Factory.StartNew(() => { });
                foreach (var split in splits)
                    task.ContinueWith((t) => this.PublishEvent(split, context));
            }
        }

        #endregion push

        #region invoke

        public override void MakeEventHandlerInvoke<TEvent>(TEvent e, EventExcutingElement element)
        {
            try
            {
                base.MakeEventHandlerInvoke(e, element);
            }
            catch (Exception ex)
            {
                this.eventRecovery.EnqueueEvent(e, element, ex);
                throw new Exception("", ex);
            }
        }

        protected override void OnSendingHandlerCommand(IEventContext context, ICommand command)
        {
            try
            {
                base.OnSendingHandlerCommand(context, command);
            }
            catch (Exception ex)
            {
                this.commandRecovery.EnqueueCommand(command, context, ex);
                throw new Exception("", ex);
            }
        }

        #endregion invoke
    }
}