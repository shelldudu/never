using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Never.Commands;
using Never.DataAnnotations;
using Never.Events;
using Never.Exceptions;
using Never.IoC;
using Never.Messages;
using Never.Reflection;
using Never.Serialization;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class AnonymousExtension
    {
        #region field

        /// <summary>
        /// 执行器缓存
        /// </summary>
        private static readonly Hashtable actionTable = null;

        /// <summary>
        /// 执行器缓存
        /// </summary>
        private static readonly Hashtable asyncActionTable = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes the <see cref="AnonymousExtension"/> class.
        /// </summary>
        static AnonymousExtension()
        {
            actionTable = new Hashtable(200);
            asyncActionTable = new Hashtable(200);
        }

        #endregion ctor

        #region command

        /// <summary>
        /// 发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        public static ICommandHandlerResult SendCommand(this ICommandBus commandBus, ICommand @command)
        {
            return SendCommand(commandBus, @command, new HandlerCommunication());
        }

        /// <summary>
        /// 发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        /// <param name="communication">上下文通讯</param>
        public static ICommandHandlerResult SendCommand(this ICommandBus commandBus, ICommand @command, HandlerCommunication communication)
        {
            if (@command == null)
            {
                throw new ParameterNullException("command", "聚合根命令不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                return commandBus.Send((dynamic)command, communication);
            }

            var type = @command.GetType();
            var action = actionTable[type] as Func<ICommandBus, ICommand, HandlerCommunication, ICommandHandlerResult>;
            if (action == null)
            {
                var emit = EasyEmitBuilder<Func<ICommandBus, ICommand, HandlerCommunication, ICommandHandlerResult>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("SendCommandBuilderInvoker", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                actionTable[type] = action = emit.CreateDelegate();
            }

            return action.Invoke(commandBus, command, communication);
        }

        /// <summary>
        /// 异步发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        public static Task<HandlerCommunication> SendCommandAsync(this ICommandBus commandBus, ICommand @command)
        {
            return SendCommandAsync(commandBus, @command, new HandlerCommunication());
        }

        /// <summary>
        /// 异步发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        /// <param name="communication">上下文通讯</param>
        public static Task<HandlerCommunication> SendCommandAsync(this ICommandBus commandBus, ICommand @command, HandlerCommunication communication)
        {
            if (@command == null)
            {
                throw new ParameterNullException("command", "聚合根命令不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                return commandBus.SendAsync((dynamic)@command, communication);
            }

            var type = @command.GetType();
            var action = asyncActionTable[type] as Func<ICommandBus, ICommand, HandlerCommunication, Task<HandlerCommunication>>;
            if (action == null)
            {
                var emit = EasyEmitBuilder<Func<ICommandBus, ICommand, HandlerCommunication, Task<HandlerCommunication>>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("SendCommandAsyncBuilderInvoker", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                asyncActionTable[type] = action = emit.CreateDelegate();
            }

            return action.Invoke(commandBus, command, communication);
        }

        /// <summary>
        /// 异步发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        /// <param name="communication">上下文通讯</param>
        private static Task<HandlerCommunication> SendCommandAsyncBuilderInvoker<TCommand>(this ICommandBus commandBus, TCommand @command, HandlerCommunication communication) where TCommand : ICommand
        {
            return commandBus.SendAsync<TCommand>(@command, communication);
        }

        /// <summary>
        /// 发布命令
        /// </summary>
        /// <param name="commandBus">命令总线</param>
        /// <param name="command">命令</param>
        /// <param name="communication">上下文通讯</param>
        private static ICommandHandlerResult SendCommandBuilderInvoker<TCommand>(this ICommandBus commandBus, TCommand @command, HandlerCommunication communication) where TCommand : ICommand
        {
            return commandBus.Send<TCommand>(@command, communication);
        }



        #endregion command

        #region event

        /// <summary>
        /// 加入事件
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="event"></param>
        public static void Push(this IEventBus eventBus, IEvent @event)
        {
            Push(eventBus, new IEvent[1] { @event }, new DefaultCommandContext());
        }

        /// <summary>
        /// 加入事件
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="context"></param>
        /// <param name="event"></param>
        public static void Push(this IEventBus eventBus, IEvent @event, ICommandContext context)
        {
            Push(eventBus, new IEvent[1] { @event }, context);
        }

        /// <summary>
        /// 加入事件
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="events"></param>
        public static void Push(this IEventBus eventBus, IEvent[] events)
        {
            Push(eventBus, events, new DefaultCommandContext());
        }

        /// <summary>
        /// 加入事件
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="context"></param>
        /// <param name="events"></param>
        public static void Push(this IEventBus eventBus, IEvent[] events, ICommandContext context)
        {
            eventBus.Push(context, new List<IEvent[]> { events });
        }


        /// <summary>
        /// 发布事件，使之能作用到目标事件
        /// </summary>
        /// <param name="eventBus">命令总线</param>
        /// <param name="e">事件</param>
        public static void PublishEvent(this IEventBus eventBus, IEvent e)
        {
            PublishEvent(eventBus, e, new DefaultCommandContext());
        }

        /// <summary>
        /// 发布事件，使之能作用到目标事件
        /// </summary>
        /// <param name="eventBus">命令总线</param>
        /// <param name="context">事件上下文</param>
        /// <param name="e">事件</param>
        public static void PublishEvent(this IEventBus eventBus, IEvent e, ICommandContext context)
        {
            if (e == null)
            {
                throw new ParameterNullException("e", "聚合根事件不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                eventBus.Publish(context, (dynamic)e);
                return;
            }

            var type = e.GetType();
            var action = actionTable[type] as Action<IEventBus, IEvent, ICommandContext>;
            if (action == null)
            {
                var emit = EasyEmitBuilder<Action<IEventBus, IEvent, ICommandContext>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("PublishEventBuilderInvoker", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                actionTable[type] = action = emit.CreateDelegate();
            }

            action.Invoke(eventBus, e, context);
        }

        /// <summary>
        /// 发布事件，使之能作用到目标事件
        /// </summary>
        /// <param name="eventBus">命令总线</param>
        /// <param name="context">事件上下文</param>
        /// <param name="e">事件</param>
        private static void PublishEventBuilderInvoker<TEvent>(this IEventBus eventBus, TEvent e, ICommandContext context) where TEvent : IEvent
        {
            eventBus.Publish(context, e);
        }

        /// <summary>
        /// 发布事件，使之能作用到目标事件
        /// </summary>
        /// <param name="eventBus">命令总线</param>
        /// <param name="context">事件上下文</param>
        /// <param name="e">事件</param>
        public static Task PublishEventAsync(this IEventBus eventBus, IEvent e, ICommandContext context)
        {
            if (e == null)
            {
                throw new ParameterNullException("e", "聚合根事件不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                return Task.FromResult(eventBus.Publish(context, (dynamic)e));
            }

            var type = e.GetType();
            var action = actionTable[type] as Func<IEventBus, IEvent, ICommandContext, Task>;
            if (action == null)
            {
                var emit = EasyEmitBuilder<Func<IEventBus, IEvent, ICommandContext, Task>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("PublishEventBuilderInvokerAsync", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                actionTable[type] = action = emit.CreateDelegate();
            }

            return action.Invoke(eventBus, e, context);
        }

        /// <summary>
        /// 发布事件，使之能作用到目标事件
        /// </summary>
        /// <param name="eventBus">命令总线</param>
        /// <param name="context">事件上下文</param>
        /// <param name="e">事件</param>
        private static Task PublishEventBuilderInvokerAsync<TEvent>(this IEventBus eventBus, TEvent e, ICommandContext context) where TEvent : IEvent
        {
            return Task.Run(() => eventBus.Publish(context, e));
        }

        #endregion event

        #region message

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="producer">消息生产者</param>
        /// <param name="object">消息对象</param>
        /// <param name="route">消息路由</param>
        public static void SendMessage<T>(this IMessageProducer producer, T @object, IMessageRoute route = null)
        {
            SendMessage<T>(producer, @object, SerializeEnvironment.JsonSerializer, route = null);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="producer">消息生产者</param>
        /// <param name="object">消息对象</param>
        /// <param name="route">消息路由</param>
        /// <param name="jsonSerilizer">json序列化对象，可以使用 Never.JsonNet.JsonNetSerializer对象 </param>
        public static void SendMessage<T>(this IMessageProducer producer, T @object, IJsonSerializer jsonSerilizer, IMessageRoute route = null)
        {
            if (@object == null)
            {
                return;
            }

            producer.Send(MessagePacket.UseJson(@object, jsonSerilizer), route);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="producer">消息生产者</param>
        /// <param name="object">消息对象</param>
        /// <param name="route">消息路由</param>
        public static Task SendMessageAsync<T>(this IMessageProducer producer, T @object, IMessageRoute route = null)
        {
            return SendMessageAsync<T>(producer, @object, SerializeEnvironment.JsonSerializer, route);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="producer">消息生产者</param>
        /// <param name="object">消息对象</param>
        /// <param name="route">消息路由</param>
        /// <param name="jsonSerilizer">json序列化对象，可以使用 Never.JsonNet.JsonNetSerializer对象 </param>
        public static Task SendMessageAsync<T>(this IMessageProducer producer, T @object, IJsonSerializer jsonSerilizer, IMessageRoute route = null)
        {
            return Task.Factory.StartNew(() =>
            {
                if (@object != null)
                {
                    producer.Send(MessagePacket.UseJson<T>(@object, jsonSerilizer), route);
                }
            });
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publisher">发布者</param>
        /// <param name="message">消息内容</param>
        public static void PublishMessage(this IMessagePublisher publisher, IMessage message)
        {
            PublishMessage(publisher, message, new DefaultMessageContext());
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publisher">发布者</param>
        /// <param name="context">消息上下文</param>
        /// <param name="message">消息内容</param>
        public static void PublishMessage(this IMessagePublisher publisher, IMessage message, IMessageContext context)
        {
            if (message == null)
            {
                throw new ParameterNullException("message", "消息内容不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                publisher.Publish(context, (dynamic)message);
                return;
            }

            var type = message.GetType();
            var action = actionTable[type] as Action<IMessagePublisher, IMessage, IMessageContext>;
            if (action == null)
            {
                var emit = EasyEmitBuilder<Action<IMessagePublisher, IMessage, IMessageContext>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("PublishMessageBuilderInvoker", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                actionTable[type] = action = emit.CreateDelegate();
            }

            action.Invoke(publisher, message, context);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publisher">发布者</param>
        /// <param name="context">消息上下文</param>
        /// <param name="message">消息内容</param>
        private static void PublishMessageBuilderInvoker<TMessage>(IMessagePublisher publisher, TMessage message, IMessageContext context) where TMessage : IMessage
        {
            publisher.Publish(context, message);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task PublishMessageAsync(this IMessagePublisher publisher, IMessage message)
        {
            return PublishMessageAsync(publisher, message, new DefaultMessageContext());
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task PublishMessageAsync(this IMessagePublisher publisher, IMessage message, IMessageContext context)
        {
            if (message == null)
            {
                throw new ParameterNullException("message", "消息内容不能为空");
            }

            if (EmitBuilder.PreferredDynamic)
            {
                return Task.FromResult(publisher.Publish(context, (dynamic)message));
            }

            var type = message.GetType();
            var function = asyncActionTable[type] as Func<IMessagePublisher, IMessage, IMessageContext, Task>;
            if (function == null)
            {
                var emit = EasyEmitBuilder<Func<IMessagePublisher, IMessage, IMessageContext, Task>>.NewDynamicMethod();
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(typeof(AnonymousExtension).GetMethod("PublishMessageAsyncBuilderInvoker", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type));
                emit.Return();
                asyncActionTable[type] = function = emit.CreateDelegate();
            }

            return function.Invoke(publisher, message, context);
        }

        /// <summary>
        /// 异步发布消息
        /// </summary>
        /// <param name="publisher">发布者</param>
        /// <param name="context">消息上下文</param>
        /// <param name="message">消息内容</param>
        private static Task PublishMessageAsyncBuilderInvoker<TMessage>(IMessagePublisher publisher, TMessage message, IMessageContext context) where TMessage : IMessage
        {
            return Task.Run(() => publisher.Publish(context, message));
        }

        #endregion

        #region ILifetimeScope

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T Resolve<T>(this ILifetimeScope scope)
        {
            return (T)scope.Resolve(typeof(T), string.Empty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Resolve<T>(this ILifetimeScope scope, string key)
        {
            return (T)scope.Resolve(typeof(T), key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T ResolveOptional<T>(this ILifetimeScope scope)
        {
            return (T)scope.ResolveOptional(typeof(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T[] ResolveAll<T>(this ILifetimeScope scope)
        {
            var all = scope.ResolveAll(typeof(T));
            var result = all?.Select(t => (T)t).ToArray();
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T GetServiceOptional<T>(this IServiceProvider serviceProvider)
        {
            try
            {
                return (T)serviceProvider.GetService(typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        #endregion ILifetimeScope

        #region validator

        /// <summary>
        /// 验证一个规则
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static ValidationResult Validate<T>(this IValidator<T> validator)
        {
            var data = validator.Validate();
            if (data.IsNullOrEmpty())
            {
                return new ValidationResult();
            }

            var list = new List<ValidationFailure>();
            foreach (var r in data)
            {
                Validator<object>.AddErrors(list, r.Key.Body, r.Value);
            }

            return new ValidationResult(list);
        }

        #endregion validator

        #region treenode

        /// <summary>
        /// 初始化树状
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="nodes">所有树节点</param>
        public static void InitTree<TNode>(this TNode root, IEnumerable<TNode> nodes) where TNode : ITreeNode<TNode>
        {
            ICollection<TNode> dictionary = new List<TNode>();
            InitTree(root, nodes, ref dictionary);
        }

        /// <summary>
        /// 初始化树状
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="nodes">所有树节点</param>
        /// <param name="collections">已根节点为开头，按深度排序的一个集合</param>
        public static void InitTree<TNode>(this TNode root, IEnumerable<TNode> nodes, ref ICollection<TNode> collections) where TNode : ITreeNode<TNode>
        {
            /*第一进入*/
            if (collections == null)
            {
                collections = new List<TNode>();
            }

            if (root == null)
            {
                return;
            }

            if (collections.Count == 0)
            {
                collections.Add(root);
            }

            if (nodes == null)
            {
                return;
            }

            /*递归方式*/
            var children = new List<TNode>();
            foreach (var n in nodes)
            {
                if (n == null)
                {
                    continue;
                }

                if (!n.ParentId.Equals(root.Id))
                {
                    continue;
                }

                children.Add(n);
            }

            if (children == null || children.Count == 0)
            {
                return;
            }

            root.Children = children;
            foreach (var child in children)
            {
                if (child == null)
                {
                    continue;
                }

                /*如果sql语句中没有为这个字段赋值,则用这个方法,可以组织出树的深度*/
                child.Level = root.Level + 1;
                child.Parent = root.Parent;
                collections.Add(child);
                InitTree(child, nodes, ref collections);
            }
        }
        #endregion
    }
}