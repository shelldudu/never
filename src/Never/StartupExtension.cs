using Never.Attributes;
using Never.Caching;
using Never.Commands;
using Never.CommandStreams;
using Never.Domains;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using Never.Messages;

#if NET461

using Never.Messages.Microstoft;

#endif

using Never.Serialization;
using Never.Startups;
using Never.Startups.Impls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Never
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static partial class StartupExtension
    {
        #region mq

#if NET461

        /// <summary>
        /// 启动msmq消费者支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="route">msmq的配置路由</param>
        /// <returns></returns>
        public static ApplicationStartup UseMSMQConsumer(this ApplicationStartup startup, MessageConnection route)
        {
            return UseMSMQConsumer(startup, route, string.Empty);
        }

        /// <summary>
        /// 启动msmq消费者支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="route">msmq的配置路由</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMSMQConsumer(this ApplicationStartup startup, MessageConnection route, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (route == null)
                throw new Exception("当前消息链接对象为空");

            if (string.IsNullOrEmpty(route.ConnetctionString))
                throw new Exception("当前消息链接对象字符串为空");

            startup.ServiceRegister.RegisterInstance(new Consumer(route), typeof(IMessageConsumer), key);
            return startup;
        }

        /// <summary>
        /// 启动msmq生产者支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="route">msmq的配置路由</param>
        /// <returns></returns>
        public static ApplicationStartup UseMSMQProducer(this ApplicationStartup startup, MessageConnection route)
        {
            return UseMSMQProducer(startup, route, string.Empty);
        }

        /// <summary>
        /// 启动msmq生产者支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="route">msmq的配置路由</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMSMQProducer(this ApplicationStartup startup, MessageConnection route, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (route == null)
                throw new Exception("当前消息链接对象为空");

            if (string.IsNullOrEmpty(route.ConnetctionString))
                throw new Exception("当前消息链接对象字符串为空");

            startup.ServiceRegister.RegisterInstance(new Producer(route), typeof(IMessageProducer), key);
            return startup;
        }

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(this ApplicationStartup startup)
        {
            return UseMemoryCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(this ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseMemoryCache" + key))
                return startup;
            startup.ServiceRegister.RegisterType(typeof(Never.Caching.MemoryCache), typeof(ICaching), key, ComponentLifeStyle.Singleton);
            startup.Items["UseMemoryCache" + key] = "t";
            return startup;
        }
#endif

        /// <summary>
        /// 启动空消息生产者支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseEmptyroducer(this ApplicationStartup startup)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseEmptyroducer"))
                return startup;

            startup.ServiceRegister.RegisterInstance(EmptyMessageProducer.Empty, typeof(IMessageProducer), string.Empty);

            startup.Items["UseEmptyroducer"] = "t";
            return startup;
        }

        #endregion mq

        #region Serializer

        /// <summary>
        /// 启动Serializer支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static ApplicationStartup UseDataContractJson(this ApplicationStartup startup, string key = "ioc.ser.datacontract")
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseDataContractJson"))
                return startup;

            if (startup.Items.ContainsKey("UseDataContractJson"))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(DataContractSerializer), typeof(IJsonSerializer), key, ComponentLifeStyle.Singleton);
            startup.Items["UseDataContractJson"] = "t";
            return startup;
        }

        /// <summary>
        /// 启动Serializer支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static ApplicationStartup UseEasyJson(this ApplicationStartup startup, string key = "ioc.ser.easyjson")
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseEasyJson"))
                return startup;

            if (startup.Items.ContainsKey("UseEasyJson"))
                return startup;

            startup.ServiceRegister.RegisterInstance(new EasyJsonSerializer(), typeof(IJsonSerializer), key);

            startup.Items["UseEasyJson"] = "t";
            return startup;
        }

        #endregion Serializer

        #region filter

        /// <summary>
        /// 创建使用正则来匹配的程序集过滤器
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static IAssemblyFilter CreateAssemblyFilter(this string pattern)
        {
            return CreateAssemblyFilter(new Regex(pattern));
        }

        /// <summary>
        /// 创建使用正则来匹配的程序集过滤器
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static IAssemblyFilter CreateAssemblyFilter(this Regex pattern)
        {
            return new RegexAssemblyFilter(pattern);
        }

        #endregion filter

        #region publish subscribe bus

        /// <summary>
        /// 启用发布订阅模式,生命周期通常声明为单例
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UsePublishSubscribeBus(this ApplicationStartup startup)
        {
            return UsePublishSubscribeBus<DefaultMessageContext>(startup, ComponentLifeStyle.Scoped);
        }

        /// <summary>
        /// 启用发布订阅模式,生命周期通常声明为单例
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UsePublishSubscribeBus(this ApplicationStartup startup, ComponentLifeStyle lifeStyle)
        {
            return UsePublishSubscribeBus<DefaultMessageContext>(startup, lifeStyle);
        }

        /// <summary>
        /// 启用发布订阅模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TMessageContext">事件上下文</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UsePublishSubscribeBus<TMessageContext>(this ApplicationStartup startup) where TMessageContext : IMessageContext
        {
            return UsePublishSubscribeBus<TMessageContext>(startup, ComponentLifeStyle.Scoped);
        }

        /// <summary>
        /// 启用发布订阅模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TMessageContext">事件上下文</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UsePublishSubscribeBus<TMessageContext>(this ApplicationStartup startup, ComponentLifeStyle lifeStyle) where TMessageContext : IMessageContext
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UsePublishSubscribeBus"))
                return startup;

            /*注册发布事件*/
            startup.ServiceRegister.RegisterType(typeof(MessagePublisher), typeof(IMessagePublisher), string.Empty, ComponentLifeStyle.Singleton);
            startup.ServiceRegister.RegisterType(typeof(TMessageContext), typeof(IMessageContext), string.Empty, ComponentLifeStyle.Transient);
            startup.RegisterStartService(new Never.Messages.StartupService(lifeStyle));
            startup.Items["UsePublishSubscribeBus"] = "t";
            return startup;
        }

        /// <summary>
        /// 使用强制检查MessageSubscriber的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckMessageSubscriberCtor(this ApplicationStartup startup)
        {
            return UseForceCheckMessageSubscriberCtor(startup, new Type[0]);
        }

        /// <summary>
        /// 使用强制检查MessageSubscriber的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="ignoreTypes">忽略的对象</param>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckMessageSubscriberCtor(this ApplicationStartup startup, IEnumerable<Type> ignoreTypes)
        {
            if (startup.Items.ContainsKey("UseForceCheckMessageSubscriberCtor"))
                return startup;

            startup.RegisterStartService(true, (x) =>
            {
                x.ProcessType(new[] { new MessageStartupService.MessageHandlerCtorProcessor(ignoreTypes) });
            });

            return startup;
        }

        #endregion publish subscribe bus

        #region inproc eventprovider commandbus

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "领域事件与发布事件分别储存")]
        public static ApplicationStartup UseInprocEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup)
            where TCommandContext : ICommandContext
        {
            return UseInprocEventProviderCommandBus<TCommandContext>(startup, EmptyEventStreamStorager.Empty);
        }

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "这个方法要提供IEventStorager接口，储存领域事件")]
        public static ApplicationStartup UseInprocEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, IEventStorager eventStorager)
            where TCommandContext : ICommandContext
        {
            return UseInprocEventProviderCommandBus<TCommandContext>(startup, eventStorager, EmptyCommandStreamStorager.Empty);
        }

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <param name="commandStorager">命令信息储存</param>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "这个方法要提供IEventStorager接口，储存领域事件")]
        [Summary(Descn = "这个方法要提供ICommandStorager接口，储存领域命令")]
        public static ApplicationStartup UseInprocEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, IEventStorager eventStorager, ICommandStorager commandStorager)
            where TCommandContext : ICommandContext
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseInprocEventProviderCommandBus"))
                return startup;

            if (startup.Items.Remove("UseMQEventProviderCommandBus"))
                throw new ArgumentNullException("系统已经注册消息模式的命令总线了，两者不同共存");

            /*注册发布事件*/
            startup.ServiceRegister.RegisterType(typeof(TCommandContext), typeof(ICommandContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterType(typeof(DefaultEventContext), typeof(IEventContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterType(typeof(InprocEventProviderCommandBus), typeof(ICommandBus), string.Empty, ComponentLifeStyle.Singleton);
            startup.ServiceRegister.RegisterInstance(eventStorager ?? EmptyEventStreamStorager.Empty, typeof(IEventStorager), string.Empty);
            startup.ServiceRegister.RegisterInstance(commandStorager ?? EmptyCommandStreamStorager.Empty, typeof(ICommandStorager), string.Empty);

            //注入handler类型的对象
            startup.UseInjectingCommandHandlerEventHandler(ComponentLifeStyle.Singleton);

            startup.Items["UseInprocEventProviderCommandBus"] = "t";
            return startup;
        }

        #endregion inproc eventprovider commandbus

        #region mq event provider commandbus

        /// <summary>
        /// 启用消息队列模式命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="messageProducerProvider">当领域对象产生一个事件的时候，如何查询出消息提供者</param>
        /// <returns></returns>
        [Summary(Descn = "领域事件与发布事件分别储存")]
        [Summary(Descn = "这个方法要提供IMessageProducerProvider接口，发布领域事件")]
        public static ApplicationStartup UseMQEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, IMessageProducerProvider messageProducerProvider) where TCommandContext : ICommandContext
        {
            return UseMQEventProviderCommandBus<TCommandContext>(startup, messageProducerProvider, EmptyEventStreamStorager.Empty);
        }

        /// <summary>
        /// 启用消息队列模式命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="messageProducerProvider">当领域对象产生一个事件的时候，如何查询出消息提供者</param>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <returns></returns>
        [Summary(Descn = "领域事件与发布事件分别储存")]
        [Summary(Descn = "这个方法要提供IMessageProducerProvider接口，发布领域事件")]
        [Summary(Descn = "这个方法要提供IEventStorager接口，储存领域事件")]
        public static ApplicationStartup UseMQEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, IMessageProducerProvider messageProducerProvider, IEventStorager eventStorager) where TCommandContext : ICommandContext
        {
            return UseMQEventProviderCommandBus<TCommandContext>(startup, messageProducerProvider, eventStorager, EmptyCommandStreamStorager.Empty);
        }

        /// <summary>
        /// 启用消息队列模式命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文</typeparam>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="messageProducerProvider">当领域对象产生一个事件的时候，如何查询出消息提供者</param>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <param name="commandStorager">命令信息储存</param>
        /// <returns></returns>
        [Summary(Descn = "领域事件与发布事件分别储存")]
        [Summary(Descn = "这个方法要提供IMessageProducerProvider接口，发布领域事件")]
        [Summary(Descn = "这个方法要提供IEventStorager接口，储存领域事件")]
        [Summary(Descn = "这个方法要提供ICommandStorager接口，储存领域命令")]
        public static ApplicationStartup UseMQEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, IMessageProducerProvider messageProducerProvider, IEventStorager eventStorager, ICommandStorager commandStorager) where TCommandContext : ICommandContext
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseMQEventProviderCommandBus"))
                return startup;

            if (startup.Items.Remove("UseInprocEventProviderCommandBus"))
                throw new ArgumentNullException("系统已经注册同进程模式的命令总线了，两者不同共存");

            /*注册发布事件*/
            startup.ServiceRegister.RegisterType(typeof(TCommandContext), typeof(ICommandContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterType(typeof(DefaultEventContext), typeof(IEventContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterInstance(messageProducerProvider ?? new DefaultMessageProducerProvider(), typeof(IMessageProducerProvider), string.Empty);
            startup.ServiceRegister.RegisterType(typeof(MQEventProviderCommandBus), typeof(ICommandBus), string.Empty, ComponentLifeStyle.Singleton);
            startup.ServiceRegister.RegisterInstance(eventStorager ?? EmptyEventStreamStorager.Empty, typeof(IEventStorager), string.Empty);
            startup.ServiceRegister.RegisterInstance(commandStorager ?? EmptyCommandStreamStorager.Empty, typeof(ICommandStorager), string.Empty);

            //注入handler类型的对象
            startup.UseInjectingCommandHandlerEventHandler(ComponentLifeStyle.Singleton);

            startup.Items["UseMQEventProviderCommandBus"] = "t";
            return startup;
        }

        #endregion mq event provider commandbus

        #region empty commandbus and eventbus

        /// <summary>
        /// 使用空的命令总线
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseEmptyCommandBus(this ApplicationStartup startup)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseEmptyCommandBus"))
                return startup;

            startup.ServiceRegister.RegisterInstance(EmptyCommandBus.Only, typeof(ICommandBus));
            startup.Items["UseEmptyCommandBus"] = "t";
            return startup;
        }

        /// <summary>
        /// 使用空的事件总线
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseEmptyEventBus(this ApplicationStartup startup)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseEmptyEventBus"))
                return startup;

            startup.ServiceRegister.RegisterInstance(EmptyEventBus.Only, typeof(IEventBus));
            startup.Items["UseEmptyEventBus"] = "t";
            return startup;
        }

        #endregion empty commandbus and eventbus

        #region eventbus

        /// <summary>
        /// 使用事件总线,通常配合CommandBus来使用
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseEventBus(this ApplicationStartup startup)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseEventBus"))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(EventBus), typeof(IEventBus), string.Empty, ComponentLifeStyle.Singleton);
            startup.Items["UseEventBus"] = "t";
            return startup;
        }

        #endregion eventbus

        #region caching

        /// <summary>
        /// 启动CounterCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseCounterCache(this ApplicationStartup startup)
        {
            return UseCounterCache(startup, string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 启动CounterCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseCounterCache(this ApplicationStartup startup, string key)
        {
            return UseCounterCache(startup, key, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 启动CounterCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseCounterCache(this ApplicationStartup startup, string key, ComponentLifeStyle lifeStyle)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseCounterCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(CounterDictCache), typeof(ICaching), key, lifeStyle);
            startup.Items["UseCounterCache" + key] = "t";

            return startup;
        }

        /// <summary>
        /// 启动ConcurrentCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseConcurrentCache(this ApplicationStartup startup)
        {
            return UseConcurrentCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动ConcurrentCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseConcurrentCache(this ApplicationStartup startup, string key)
        {
            return UseConcurrentCache(startup, key, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 启动ConcurrentCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseConcurrentCache(this ApplicationStartup startup, string key, ComponentLifeStyle lifeStyle)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseConcurrentCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(ConcurrentCounterDictCache), typeof(ICaching), key, lifeStyle);
            startup.Items["UseConcurrentCache" + key] = "t";

            return startup;
        }

        /// <summary>
        /// 启动ThreadContextCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseThreadContextCache(this ApplicationStartup startup)
        {
            return UseThreadContextCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动ThreadContextCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseThreadContextCache(this ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseThreadContextCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(ThreadContextCache), typeof(ICaching), key, ComponentLifeStyle.Scoped);
            startup.Items["UseThreadContextCache" + key] = "t";
            return startup;
        }

        #endregion caching

        #region command and event

        /// <summary>
        /// 注入commandhanler和eventhandler
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static ApplicationStartup UseInjectingCommandHandlerEventHandler(this ApplicationStartup startup, ComponentLifeStyle lifeStyle)
        {
            if (startup.Items.ContainsKey("UseInjectCommandHandlerEventHandler"))
                return startup;

            startup.RegisterStartService(new Never.Domains.StartupService(lifeStyle));
            return startup;
        }

        /// <summary>
        /// 使用强制检查命令是否带上了<see cref="Never.CommandStreams.CommandDomainAttribute"/>特性
        /// </summary>
        /// <param name="startup">The startup.</param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckCommandAppDomainAttribute(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseForceCheckCommandAppDomainAttribute"))
                return startup;

            startup.RegisterStartService(new CommandAppDomainStartupService());
            return startup;
        }

        /// <summary>
        /// 使用强制检查事件是否带上了<see cref="Never.EventStreams.EventAppDomainStartupService"/>特性
        /// </summary>
        /// <param name="startup">The startup.</param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckEventAppDomainAttribute(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseForceCheckEventAppDomainAttribute"))
                return startup;

            startup.RegisterStartService(new EventAppDomainStartupService());
            return startup;
        }

        /// <summary>
        /// 使用强制聚合根有Handle方法但没有实现<see cref="Never.Domains.IHandle{TEvent}"/>接口
        /// </summary>
        /// <param name="startup">The startup.</param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckAggregateRootImplIHandle(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseForceCheckAggregateRootImplIHandle"))
                return startup;

            startup.RegisterStartService(new AggregateRootIHandleStartService());
            return startup;
        }

        /// <summary>
        /// 使用强制检查CommandHandler的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckCommandHandlerCtor(this ApplicationStartup startup)
        {
            return UseForceCheckCommandHandlerCtor(startup, new Type[0]);
        }

        /// <summary>
        /// 使用强制检查CommandHandler的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="ignoreTypes">忽略的对象</param>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckCommandHandlerCtor(this ApplicationStartup startup, IEnumerable<Type> ignoreTypes)
        {
            if (startup.Items.ContainsKey("UseForceCheckCommandHandlerCtor"))
                return startup;

            startup.RegisterStartService(true, (x) =>
            {
                x.ProcessType(new[] { new CommandStartupService.CommandHandlerCtorParameterProcessor(ignoreTypes ?? Type.EmptyTypes) });
            });
            return startup;
        }

        /// <summary>
        /// 使用强制检查EventHandler的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckEventHandlerCtor(this ApplicationStartup startup)
        {
            return UseForceCheckEventHandlerCtor(startup, new Type[0]);
        }

        /// <summary>
        /// 使用强制检查EventHandler的构造函数是否在IoC可以构造出来
        /// </summary>
        /// <param name="ignoreTypes">忽略的对象</param>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckEventHandlerCtor(this ApplicationStartup startup, IEnumerable<Type> ignoreTypes)
        {
            if (startup.Items.ContainsKey("UseForceCheckEventHandlerCtor"))
                return startup;

            startup.RegisterStartService(true, (x) => { x.ProcessType(new[] { new EventStartupService.EventHandlerCtorParameterProcessor(ignoreTypes ?? Type.EmptyTypes) }); });
            return startup;
        }

        /// <summary>
        /// 使用强制检查Event的是存在没有参数的构造函数
        /// </summary>
        /// <param name="ignoreTypes">忽略的对象</param>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseForceCheckCommandEvenWithNoParamaterCtor(this ApplicationStartup startup, IEnumerable<Type> ignoreTypes = null)
        {
            if (startup.Items.ContainsKey("UseForceCheckEventHandlerCtor"))
                return startup;

            startup.RegisterStartService(int.MaxValue, (x) => { x.ProcessType(new[] { new CommandStartupService.CommandNoCtorParamaterProcessor(ignoreTypes ?? Type.EmptyTypes) }); });
            startup.RegisterStartService(int.MaxValue, (x) => { x.ProcessType(new[] { new EventStartupService.EventNoCtorParamaterProcessor(ignoreTypes ?? Type.EmptyTypes) }); });
            return startup;
        }

        #endregion command and event

        #region ioc

        /// <summary>
        /// 在sampleioc中自动使用属性发现注入
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static ApplicationStartup UseAutoInjectingAttributeUsingIoC(this ApplicationStartup startup, IAutoInjectingEnvironmentProvider[] providers)
        {
            startup.RegisterStartService(new AutoInjectingStartupService(providers));
            return startup;
        }

        #endregion ioc

        #region remote

        /// <summary>
        /// 【客户端】开户请求远程调用
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="key"></param>
        /// <param name="serverEndPoint"></param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRemoteRequestHandler(this ApplicationStartup startup, string key, System.Net.EndPoint serverEndPoint)
        {
            if (startup.Items.ContainsKey(string.Concat("UseHttpRemoteRequestHandler", key)))
                return startup;

            var client = new Remoting.ClientRequestHadler(serverEndPoint, new Remoting.Http.Protocol());
            startup.ServiceRegister.RegisterInstance(client, typeof(Remoting.IRequestHandler), key);
            startup.RegisterStartService(true, (x) => { client.Startup(); });

            startup.Items[string.Concat("UseHttpRemoteRequestHandler", key)] = "t";
            return startup;
        }

        /// <summary>
        /// 【服务端】开户处理远程调用
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="handler"></param>
        /// <param name="listeningEndPoint"></param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRemoteResponseHandler(this ApplicationStartup startup, System.Net.EndPoint listeningEndPoint, Remoting.IResponseHandler handler)
        {
            if (startup.Items.ContainsKey("UseHttpRemoteResponseHandler"))
                return startup;

            var service = new Remoting.ServerRequestHandler(listeningEndPoint, handler, new Remoting.Http.Protocol());
            startup.RegisterStartService(20, (x) => { service.Startup(); });

            startup.Items["UseHttpRemoteResponseHandler"] = "t";
            return startup;
        }

        #endregion
    }
}