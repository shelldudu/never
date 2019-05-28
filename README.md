never是个开发框架，它的宗旨是让net开发简单。

所包含如下工具：

－ioc 使用emit实现构造参数方式实现ioc功能

－aop 可动态生成类，动态方法，实现友好的功能

－jsonser 序列化与反序列化都有高性能的改善

－ sqlclient 使用emit减少反射损耗，api接口友好

－ cqrs 使用commandbus+eventbus快速实现一套cqrs开发效率高，要求使用ddd技术

－ valiator 参数验证，属性验证方式，自由编写自己的验证规则

－ mapper 快速实现属性与字段之间的映射

－ sockets 使用SocketAsyncEvent封装的高性能socket，衍生出配置中心

－ easysql 与ibatis相似，但省去更多配置，使用方便有极大提升，同时支持typehander，自由配置不同类型字段的读取

－ deployment 动态生成httprequest请求，并支持熔断技术，让业务开发者直接调用service接口变成本地调用方法般简单

－ worlflow 简单工作流


组件初始流程：

```
private void Startup_OnStarting(object sender, Never.StartupEventArgs e)
{
    e.Startup.RegisterAssemblyFilter("Never".CreateAssemblyFilter())
    .UseEasyIoC((x,y,z)=>{})
    .UseCounterCache()
    .UseConcurrentCache()
    .UseDataContractJson()
    .UseEasyJson(string.Empty)
    .UseNLog(logfile)
    .UseMvcActionCustomRoute(e.Collector as IServiceCollection)
    .UseMvcModelStateValidation()
    .UseForceCheckAggregateRootImplIHandle()
    .UseForceCheckCommandAppDomainAttribute()
    .UseForceCheckCommandEvenWithNoParamaterCtor()
    .UseForceCheckCommandHandlerCtor()
    .UseForceCheckEventAppDomainAttribute()
    .UseForceCheckEventHandlerCtor()
    .UseForceCheckMessageSubscriberCtor()
    .UseAppConfig(configReader)
    .UseMvcPermission()
    .UseApiUriRouteDispatch(40, (x) => new IApiRouteProvider[]
    {
        new B2C.Message.Contract.Services.ApiRouteProvider(configReader),
    }, () => e.Startup.ServiceLocator.ResolveOptional<ILoggerBuilder>())
    .UseHttpProxyGenerateMessageApi()
    .UseInjectingCommandHandlerEventHandler(Never.IoC.ComponentLifeStyle.Singleton)
    .UseSqliteEventProviderCommandBus<CommandContextWrapper>(new SqliteFailRecoveryStorager(commandfile, eventfile))
    .UseMvcDependency(e.Collector as IServiceCollection);
}
```

详情可看 https://github.com/shelldudu/never_application
