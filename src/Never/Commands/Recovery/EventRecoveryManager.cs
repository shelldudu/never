using Never.Events;
using Never.IoC;
using Never.Logging;
using System;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 用于协助Command发布，失败后重试的机制
    /// </summary>
    public class EventRecoveryManager : Never.Threading.ThreadCircler
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IFailRecoveryStorager storager = null;

        /// <summary>
        ///
        /// </summary>
        private ILoggerBuilder loggerBuilder = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IEventBus eventBus = null;

        /// <summary>
        ///
        /// </summary>
        private readonly ICommandBus commandBus = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IServiceLocator serviceLocator = null;

        /// <summary>
        /// 睡眠时间
        /// </summary>
        private readonly TimeSpan sleepTimeSpan = TimeSpan.Zero;
        #endregion field

        #region ctor

        /// <summary>
        /// dozeTime = TimeSpan.FromMinutes(1)表示1秒发一个命令，如果还有命令，则一直发（间隔为1秒），此时RepeatWork返回false
        /// waitTime = TimeSpan.Zero 当任务完成，则进入睡眠状态，直到被唤醒，此时RepeatWork返回true
        /// </summary>
        /// <param name="storager"></param>
        /// <param name="eventBus"></param>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        public EventRecoveryManager(IFailRecoveryStorager storager, IServiceLocator serviceLocator, ICommandBus commandBus, IEventBus eventBus)
            : this(storager, serviceLocator, commandBus, eventBus, null)
        {
        }

        /// <summary>
        /// dozeTime = TimeSpan.FromMinutes(1)表示1秒发一个命令，如果还有命令，则一直发（间隔为1秒），此时RepeatWork返回false
        /// waitTime = TimeSpan.Zero 当任务完成，则进入睡眠状态，直到被唤醒，此时RepeatWork返回true
        /// </summary>
        /// <param name="storager"></param>
        /// <param name="eventBus"></param>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        /// <param name="loggerBuilder"></param>
        protected EventRecoveryManager(IFailRecoveryStorager storager, IServiceLocator serviceLocator, ICommandBus commandBus, IEventBus eventBus, ILoggerBuilder loggerBuilder) : base(null, typeof(EventRecoveryManager).Name)
        {
            this.storager = storager;
            this.serviceLocator = serviceLocator;
            this.eventBus = eventBus;
            this.commandBus = commandBus;
            if (storager is IWorkTigger && ((IWorkTigger)storager).Timer != TimeSpan.Zero)
                this.sleepTimeSpan = ((IWorkTigger)storager).Timer;

            if (this.sleepTimeSpan == TimeSpan.Zero)
                this.sleepTimeSpan = TimeSpan.FromSeconds(10);

            this.Replace(Change).Start();
        }

        #endregion ctor

        #region BaseThreadCircler

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected TimeSpan Change()
        {
            var model = this.DequeueEvent();
            /*任务完成，打个瞌睡，n秒后继续跑*/
            if (model == null)
                return this.sleepTimeSpan;

            ICommandContext context = null;
            try
            {
                this.PublicEvent(model, out context);
            }
            catch (Exception ex)
            {
                this.EnqueueEvent(model.Event, model.EventHandlerType, context, ex.GetInnerException());
            }
            finally
            {
            }

            /*任务未完成，打个瞌睡，n秒后继续跑*/
            return this.sleepTimeSpan;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected override void HandleException(Exception ex, string message)
        {
            if (this.loggerBuilder != null)
            {
                this.loggerBuilder.Build(typeof(EventRecoveryManager)).Error(message, ex);
                return;
            }

            using (var scope = this.serviceLocator.BeginLifetimeScope())
            {
                this.loggerBuilder = scope.ResolveOptional<ILoggerBuilder>();
                this.loggerBuilder.Build(typeof(EventRecoveryManager)).Error(message, ex);
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        protected virtual void PublicEvent(RecoveryEventModel model, out ICommandContext context)
        {
            context = this.GetCommandContext(model);
            AnonymousExtension.PublishEvent(new FilterEventBus(model.EventHandlerType, this.serviceLocator, this.commandBus), model.Event, context);
        }

        /// <summary>
        /// 初始化上下文
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual ICommandContext GetCommandContext(RecoveryEventModel model)
        {
            var context = new DefaultCommandContext()
            {
                Worker = new RecoveryWorker()
            };

            return context;
        }

        #endregion BaseThreadCircler

        #region IFailRecoveryStorager

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="element">执行元素</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        public void EnqueueEvent(IEvent @event, EventExcutingElement element, Exception exception)
        {
            this.storager.Enqueue(element.EventContext, exception, @event, element.EventHandlerType);
            if (this.IsWaited)
                this.Pulse();
        }
        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="context">执行元素</param>
        /// <param name="handlerType">执行元素</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        public void EnqueueEvent(IEvent @event, Type handlerType, ICommandContext context, Exception exception)
        {
            this.storager.Enqueue(context, exception, @event, handlerType);
            if (this.IsWaited)
                this.Pulse();
        }
        /// <summary>
        /// 弹出一个命令
        /// </summary>
        /// <returns></returns>
        public RecoveryEventModel DequeueEvent()
        {
            return this.storager.DequeueEvent();
        }

        #endregion IFailRecoveryStorager

        #region eventbus

        private class FilterEventBus : EventBus, IEventBus
        {
            private readonly Type eventHandlerType = null;

            public FilterEventBus(Type eventHandlerType, IServiceLocator serviceLocator, ICommandBus commandBus) : base(commandBus, serviceLocator)
            {
                this.eventHandlerType = eventHandlerType;
            }

            protected override bool ExcutingElementFilter(EventExcutingElement element)
            {
                if (element.EventHandlerType != this.eventHandlerType)
                    return false;

                return base.ExcutingElementFilter(element);
            }
        }

        #endregion eventbus
    }
}