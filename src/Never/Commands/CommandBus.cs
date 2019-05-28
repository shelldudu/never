using Never.Aop;
using Never.Aop.DomainFilters;
using Never.CommandStreams;
using Never.DataAnnotations;
using Never.Domains;
using Never.Events;
using Never.EventStreams;
using Never.Exceptions;
using Never.IoC;
using Never.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Never.Commands
{
    /// <summary>
    /// 调度管理器
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CommandBus : ICommandBus
    {
        #region field

        /// <summary>
        /// ioc管理
        /// </summary>
        protected readonly IServiceLocator serviceLocator = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus"/> class.
        /// </summary>
        /// <param name="serviceLocator">服务定位器</param>
        protected CommandBus(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        #endregion ctor

        #region ICommandBus成员

        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="command">信息</param>
        public virtual ICommandHandlerResult Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            return this.Send<TCommand>(command, new HandlerCommunication());
        }

        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="command">信息</param>
        /// <param name="communication">上下文通讯</param>
        public virtual ICommandHandlerResult Send<TCommand>(TCommand command, HandlerCommunication communication) where TCommand : ICommand
        {
            var commandType = command.GetType();
            this.OnCommandValidating(command, commandType);

            if (communication == null)
                communication = new HandlerCommunication();

            var element = this.FindCommandExcutingElement(command, communication);

            try
            {
                /*先保存命令*/
                this.HandlerCommandToStorage(element.CommandContext, command);
                ICommandHandlerResult handlerResult = null;
                var roots = this.HanderCommand(element, command, communication, ref handlerResult);
                if (roots == null || roots.Length == 0)
                {
                    communication.HandlerResult = handlerResult;
                    return handlerResult;
                }

                if (handlerResult != null && handlerResult.Status != CommandHandlerStatus.Success)
                {
                    communication.HandlerResult = handlerResult;
                    return handlerResult;
                }

                var eventArray = new List<KeyValuePair<Type, IEvent[]>>(roots.Length);
                var eventSource = new List<IEvent[]>(roots.Length);
                foreach (var root in roots)
                {
                    var array = root.Change(root.Version + root.GetChangeCounts());
                    eventSource.Add(array);
                    var item = new KeyValuePair<Type, IEvent[]>(root.GetType(), array);
                    eventArray.Add(item);
                }

                /*先保存事件*/
                this.HandlerEventToStorage(element.CommandContext, eventArray);

                /*再发布事件*/
                this.HandlerEventToPublish(element.CommandContext, eventSource);

                communication.HandlerResult = handlerResult;
                return handlerResult;
            }
            catch
            {
                throw;
            }
            finally
            {
                OnReturningWhenHandlerExecuted(element.CommandContext);
            }
        }

        /// <summary>
        /// 异步发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">命令信息</param>
        public async virtual Task<HandlerCommunication> SendAsync<TCommand>(TCommand c) where TCommand : ICommand
        {
            return await this.SendAsync(c, new HandlerCommunication());
        }

        /// <summary>
        /// 异步发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="c">信息</param>
        /// <param name="communication">上下文通讯</param>
        public async virtual Task<HandlerCommunication> SendAsync<TCommand>(TCommand c, HandlerCommunication communication) where TCommand : ICommand
        {
            return await Task<HandlerCommunication>.Factory.StartNew(() => { this.Send(c, communication); return communication; });
        }

        #endregion ICommandBus成员

        #region invoke

        /// <summary>
        /// 验证全集参数规则
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <param name="commandType"></param>
        protected virtual void OnCommandValidating<TCommand>(TCommand command, Type commandType) where TCommand : ICommand
        {
            /*新加命令参数验证*/
            var validator = this.CommandValidate(command, commandType);
            if (!validator.IsValid)
                throw new ParameterException(validator.Errors[0].MemberName, validator.Errors[0].ErrorMessage);
        }

        /// <summary>
        /// 命令是否通过
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="command">命令信息</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected virtual ValidationResult CommandValidate<TCommand>(TCommand command, Type commandType) where TCommand : ICommand
        {
            /*新加命令参数验证*/
            var validator = command as IValidator<TCommand>;
            if (validator != null)
                return AnonymousExtension.Validate(validator);

            var commandAttribute = CommandBehaviorStorager.Default.GetAttribute<ValidatorAttribute>(commandType);
            if (commandAttribute != null && commandAttribute.ValidatorType != null)
            {
                using (var scope = this.serviceLocator.BeginLifetimeScope())
                {
                    var ivalidator = scope.ResolveOptional(commandAttribute.ValidatorType) as IValidator;
                    if (ivalidator != null)
                        return ivalidator.Validate(command);
                }
            }

            return default(ValidationResult);
        }

        /// <summary>
        /// 发布命令,命令只允许一个消费者,如果存在多个,则会有异常
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="element">命令元素</param>
        /// <param name="command">信息</param>
        /// <param name="communication">上下文通讯</param>
        /// <param name="handlerResult">处理结果</param>
        protected IAggregateRoot[] HanderCommand<TCommand>(CommandExcutingElement element, TCommand command, HandlerCommunication communication, ref ICommandHandlerResult handlerResult) where TCommand : ICommand
        {
            if (element.CommandHandler == null)
                return new IAggregateRoot[] { };

            if (element.AuthorizeFilters != null)
            {
                foreach (var filter in element.AuthorizeFilters)
                {
                    if (!filter.Validate(element.CommandContext, command))
                        return new IAggregateRoot[] { };
                }
            }

            if (element.LoggerBuilder == null)
            {
                handlerResult = this.MarkCommandHandlerInvoke(command, element, communication);
                foreach (var t in element.CommandContext.Items)
                {
                    communication[t.Key] = t.Value;
                }
            }
            else
            {
                element.CommandContext.Items["LoggerBuilder"] = element.LoggerBuilder;
                try
                {
                    handlerResult = this.MarkCommandHandlerInvoke(command, element, communication);
                    foreach (var t in element.CommandContext.Items)
                    {
                        communication[t.Key] = t.Value;
                    }
                }
                catch (ParameterException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new ParameterException(ex.ParameterName, "处理出错，请稍后重试", ex);
                }
                catch (InvalidException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new InvalidException("处理出错，请稍后重试", ex);
                }
                catch (DomainException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new DomainException("处理出错，请稍后重试", ex);
                }
                catch (StreamStorageException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new StreamStorageException(ex.Message, ex);
                }
                catch (RepositoryExcutingException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new RepositoryExcutingException(ex.Message, ex);
                }
                catch (RepeatExcutingException ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new RepeatExcutingException(ex.Message, ex);
                }
                catch (ResourcePoorException ex)
                {
                    throw new ResourcePoorException(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    this.OnCommandHandlerError(element, command, ex);
                    throw new Exception(ex.Message, ex);
                }
            }

            /*处理聚合根事件*/
            var roots = element.CommandContext.GetChangeAggregateRoot();
            if (roots == null || roots.Length <= 0)
                return new IAggregateRoot[] { };

            var list = new List<IAggregateRoot>(roots.Length);
            foreach (var root in roots)
            {
                if (root.CanCommit())
                    list.Add(root);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 找出当前的命令信息
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="command">信息</param>
        /// <param name="communication">上下文通讯</param>
        /// <returns></returns>
        protected CommandExcutingElement FindCommandExcutingElement<TCommand>(TCommand command, HandlerCommunication communication) where TCommand : ICommand
        {
            var provider = new CommandHandlerProvider(this.serviceLocator.BeginLifetimeScope());
            var element = CommandBusExcutingHelper.FindCommandExcutingElement(provider, command, communication);
            communication["BeginLifetimeScope"] = provider.Scope;
            return element;
        }

        /// <summary>
        /// 释放由<see cref="IServiceLocator"/>创建的对象
        /// </summary>
        /// <param name="context"></param>
        protected void Release(HandlerCommunication context)
        {
            var scope = context.ContainsKey("BeginLifetimeScope") ? context["BeginLifetimeScope"] as ILifetimeScope : null;
            if (scope != null)
            {
                try
                {
                    scope.Dispose();
                    context.Remove("BeginLifetimeScope");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 生成方法
        /// </summary>
        /// <typeparam name="TCommand">信息类型</typeparam>
        /// <param name="command">信息</param>
        /// <param name="element">命令执行元素</param>
        /// <param name="communication"></param>
        /// <returns></returns>
        protected ICommandHandlerResult MarkCommandHandlerInvoke<TCommand>(TCommand command, CommandExcutingElement element, HandlerCommunication communication) where TCommand : ICommand
        {
            try
            {
                foreach (var handler in element.HandlerFilters)
                {
                    handler.OnActionExecuting(element.CommandContext, command);
                }

                foreach (var handler in element.ExcuteFilters)
                {
                    handler.OnActionExecuting(element.CommandContext, command);
                }

                /*处理事件*/
                return ((ICommandHandler<TCommand>)element.CommandHandler).Execute(element.CommandContext, command);
            }
            catch
            {
                throw;
            }
            finally
            {
                Release(communication);
            }
        }

        /// <summary>
        /// 在完成处理命令的时候即将return的时候回调的方法
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnReturningWhenHandlerExecuted(ICommandContext context)
        {
        }

        #endregion invoke

        #region error

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="element">执行元素</param>
        /// <param name="command">命令</param>
        /// <param name="ex">异常信息</param>
        protected void OnCommandHandlerError<TCommand>(CommandExcutingElement element, TCommand command, Exception ex) where TCommand : ICommand
        {
            if (element.LoggerAttribute == null)
                return;

            try
            {
                element.LoggerAttribute.OnError(element.CommandContext, element.LoggerBuilder, element.CommandHandler, ex, element.CommandContext);
            }
            catch
            {
            }
            finally
            {
            }
        }

        #endregion error

        #region root

        /// <summary>
        /// 保存处理命令
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="command">命令</param>
        protected virtual void HandlerCommandToStorage<TCommand>(ICommandContext commandContext, TCommand command) where TCommand : ICommand
        {
        }

        /// <summary>
        /// 保存处理事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="eventArray">事件源</param>
        protected virtual void HandlerEventToStorage(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> eventArray)
        {
        }

        /// <summary>
        /// 处理事件，主要是发布事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="eventArray">事件源</param>
        /// <returns></returns>
        protected virtual void HandlerEventToPublish(ICommandContext commandContext, IEnumerable<IEvent[]> eventArray)
        {
        }

        #endregion root
    }
}