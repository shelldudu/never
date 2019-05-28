using Never.Events;
using Never.IoC;
using Never.Logging;
using System;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 用于协助Command发布，失败后重试的机制
    /// </summary>
    public class CommandRecoveryManager : Never.Threading.ThreadCircler
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
        /// dozeTime = TimeSpan.FromMinutes(1)表示1分发一个命令，如果还有命令，则一直发（间隔为1秒），此时RepeatWork返回false
        /// waitTime = TimeSpan.Zero 当任务完成，则进入睡眠状态，直到被唤醒，此时RepeatWork返回true
        /// </summary>
        /// <param name="storager"></param>
        /// <param name="commandBus"></param>
        /// <param name="eventBus"></param>
        /// <param name="serviceLocator"></param>
        public CommandRecoveryManager(IFailRecoveryStorager storager, IServiceLocator serviceLocator, ICommandBus commandBus, IEventBus eventBus)
            : this(storager, serviceLocator, commandBus, eventBus, null)
        {
        }

        /// <summary>
        /// dozeTime = TimeSpan.FromMinutes(1)表示1分发一个命令，如果还有命令，则一直发（间隔为1秒），此时RepeatWork返回false
        /// waitTime = TimeSpan.Zero 当任务完成，则进入睡眠状态，直到被唤醒，此时RepeatWork返回true
        /// </summary>
        /// <param name="storager"></param>
        /// <param name="commandBus"></param>
        /// <param name="eventBus"></param>
        /// <param name="serviceLocator"></param>
        /// <param name="loggerBuilder"></param>
        public CommandRecoveryManager(IFailRecoveryStorager storager, IServiceLocator serviceLocator, ICommandBus commandBus, IEventBus eventBus, ILoggerBuilder loggerBuilder) : base(null, typeof(CommandRecoveryManager).Name)
        {
            this.storager = storager;
            this.commandBus = commandBus;
            this.eventBus = eventBus;
            this.serviceLocator = serviceLocator;
            this.loggerBuilder = loggerBuilder;
            if (storager is IWorkTigger && ((IWorkTigger)storager).Timer != TimeSpan.Zero)
                this.sleepTimeSpan = ((IWorkTigger)storager).Timer;

            if (this.sleepTimeSpan == TimeSpan.Zero)
                this.sleepTimeSpan = TimeSpan.FromSeconds(10);

            this.Replace(Change).Start();
        }

        #endregion ctor

        #region AbstThreadCircling

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected TimeSpan Change()
        {
            var model = this.DequeueCommand();
            /*任务完成，打个瞌睡，10秒后继续跑*/
            if (model == null)
                return this.sleepTimeSpan;

            HandlerCommunication communication = null;
            try
            {
                this.PublicCommand(model, out communication);
            }
            catch (Exception ex)
            {
                this.EnqueueCommand(model.Command, communication, ex.GetInnerException());
            }
            finally
            {
            }

            /*任务未完成，打个瞌睡，10秒后继续跑*/
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
        /// <param name="communication"></param>
        protected virtual void PublicCommand(RecoveryCommandModel model, out HandlerCommunication communication)
        {
            communication = this.GetCommandContext(model);
            AnonymousExtension.SendCommand(this.commandBus, model.Command, communication);
        }

        /// <summary>
        /// 初始化上下文
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual HandlerCommunication GetCommandContext(RecoveryCommandModel model)
        {
            var context = new HandlerCommunication()
            {
                Worker = new RecoveryWorker() { AdditionId = model.Id, AdditionGuid = model.UniqueId }
            };

            return context;
        }

        #endregion AbstThreadCircling

        #region IFailRecoveryStorager

        /// <summary>
        /// 保存一个命令，用于重试
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="command"></param>
        /// <param name="exception">异常</param>
        public void EnqueueCommand(ICommand command, IEventContext context, Exception exception)
        {
            this.storager.Enqueue(context, exception, command);
            if (this.IsWaited)
                this.Pulse();
        }

        /// <summary>
        /// 保存一个命令，用于重试
        /// </summary>
        /// <param name="communication">上下文</param>
        /// <param name="command"></param>
        /// <param name="exception">异常</param>
        public void EnqueueCommand(ICommand command, HandlerCommunication communication, Exception exception)
        {
            this.storager.Enqueue(communication, exception, command);
            if (this.IsWaited)
                this.Pulse();
        }

        /// <summary>
        /// 弹出一个命令
        /// </summary>
        /// <returns></returns>
        public RecoveryCommandModel DequeueCommand()
        {
            return this.storager.DequeueCommand();
        }

        #endregion IFailRecoveryStorager
    }
}