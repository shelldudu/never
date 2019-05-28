using Never.Commands;
using Never.Exceptions;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Never.Events
{
    /// <summary>
    /// 发布事件
    /// </summary>
    public class EventBus : IEventBus
    {
        #region field

        /// <summary>
        /// ioc管理
        /// </summary>
        private readonly IServiceLocator serviceLocator = null;

        /// <summary>
        /// 命令总线
        /// </summary>
        private readonly ICommandBus commandBus = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        public EventBus(ICommandBus commandBus,
            IServiceLocator serviceLocator)
        {
            this.commandBus = commandBus;
            this.serviceLocator = serviceLocator;
        }

        #endregion ctor

        #region push

        /// <summary>
        /// 将事件压入队列
        /// </summary>
        /// <param name="context"></param>
        /// <param name="events"></param>
        public virtual void Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
            if (events == null)
                return;

            foreach (var splits in events)
            {
                if (splits.IsNullOrEmpty())
                    continue;

                foreach (var split in splits)
                    this.PublishEvent(split, context);
            }
        }

        #endregion push

        #region publish

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">信息类型</typeparam>
        /// <param name="commandContext">上下文通讯</param>
        /// <param name="e">信息</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Publish<TEvent>(ICommandContext commandContext, TEvent e)
            where TEvent : IEvent
        {
            if (commandContext == null)
                commandContext = new DefaultCommandContext();

            var elements = this.FindEventExcutingElement(commandContext, e).FindAll(t => this.ExcutingElementFilter(t));
            if (elements == null || elements.Count == 0)
                return;

            foreach (var element in elements)
            {
                /*默认的事件上下文*/
                var defaultEventContext = element.EventContext as IStartupEventContext;
                /*初始化事件上下文*/
                if (defaultEventContext != null)
                    defaultEventContext.OnInit(commandContext);
                if (element.LoggerBuilder != null)
                    element.EventContext.Items["LoggerBuilder"] = element.LoggerBuilder;
            }

            foreach (var element in elements)
            {
                try
                {
                    this.MakeEventHandlerInvoke(e, element);
                    this.HandlerCommandToPublish(element.EventContext);
                }
                catch (Exception ex)
                {
                    this.OnEventHandlerError(element, e, ex);
                }
                finally
                {
                    this.OnReturningWhenHandlerExecuted(element.EventContext);
                }
            }

            this.Release(commandContext);
        }

        #endregion publish

        #region filter

        /// <summary>
        /// 过滤执行元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual bool ExcutingElementFilter(EventExcutingElement element)
        {
            return true;
        }

        #endregion filter

        #region invoke

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="commandContext"></param>
        protected void Release(ICommandContext commandContext)
        {
            var scope = commandContext.Items != null && commandContext.Items.ContainsKey("BeginLifetimeScope") ? commandContext.Items["BeginLifetimeScope"] as ILifetimeScope : null;
            if (scope != null)
            {
                try
                {
                    scope.Dispose();
                    commandContext.Items.Remove("BeginLifetimeScope");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 生成方法
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="e">事件</param>
        /// <param name="element">执行元素</param>
        public virtual void MakeEventHandlerInvoke<TEvent>(TEvent e, EventExcutingElement element) where TEvent : IEvent
        {
            foreach (var handler in element.HandlerFilters)
            {
                handler.OnActionExecuting(element.EventContext, e);
            }

            ((IEventHandler<TEvent>)element.EventHandler).Execute(element.EventContext, e);
        }

        /// <summary>
        /// 处理事件，主要是发布命令
        /// </summary>
        /// <param name="context">命令上下文</param>
        /// <returns></returns>
        protected void HandlerCommandToPublish(IEventContext context)
        {
            var commands = context.GetAllCommands();
            if (commands == null)
                return;

            foreach (var command in commands)
            {
                try
                {
                    this.OnSendingHandlerCommand(context, command);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        protected virtual void OnSendingHandlerCommand(IEventContext context, ICommand command)
        {
            AnonymousExtension.SendCommand(this.commandBus, command, new HandlerCommunication(context));
        }

        /// <summary>
        /// 在完成处理命令的时候即将return的时候回调的方法
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnReturningWhenHandlerExecuted(IEventContext context)
        {
        }

        #endregion invoke

        #region error

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="element">执行元素</param>
        /// <param name="e">事件</param>
        /// <param name="ex">异常</param>
        public void OnEventHandlerError<TEvent>(EventExcutingElement element, TEvent e, Exception ex) where TEvent : IEvent
        {
            if (element.LoggerAttribute == null)
                return;

            try
            {
                element.LoggerAttribute.OnError(element.EventContext, element.LoggerBuilder, element.EventHandlerType, ex, element.EventContext);
            }
            catch
            {
            }
            finally
            {
            }
        }

        #endregion error

        #region utils

        /// <summary>
        /// 查询执行元素
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="e">事件</param>
        /// <returns></returns>
        private List<EventExcutingElement> FindEventExcutingElement<TEvent>(ICommandContext commandContext, TEvent e)
            where TEvent : IEvent
        {
            try
            {
                var provider = new EventHandlerProvider(this.serviceLocator.BeginLifetimeScope());
                var elements = EventBusExcutingHelper.FindEventExcutingElement(provider, commandContext, e);
                commandContext.Items["BeginLifetimeScope"] = provider.Scope;
                return elements;
            }
            catch
            {
                throw;
            }
        }

        #endregion utils
    }
}