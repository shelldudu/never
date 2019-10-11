using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Never.EventStreams;
using Never.IoC;
using Never.WorkerService;
using Never.WorkFlow.Test.Messages;
using Never.WorkFlow.Test.Steps;
using static Never.WorkFlow.Test.FlowMain;

namespace Never.WorkFlow.Test
{
    class Startup : Never.WorkerService.WorkerStartup
    {
        public Startup() : base(() => new Never.ApplicationStartup())
        {
            this.OnStarting += this.Startup_OnStarting;
        }

        private void Startup_OnStarting(object sender, Never.StartupEventArgs e)
        {
            e.Startup.RegisterAssemblyFilter("Never".CreateAssemblyFilter())
                .UseEasyIoC((x, y, z) =>
                {
                    var types = y.FindClassesOfType<IWorkStep>(z.ToArray());
                    foreach (var type in types)
                    {
                        x.RegisterType(type, type);
                    }
                })
                .UseForceCheckCommandHandlerCtor()
                .UseForceCheckEventHandlerCtor()
                .UseDataContractJson()
                .UseEasyJson()
                .UseAutoInjectingAttributeUsingIoC(new[]
                {
                    SingletonAutoInjectingEnvironmentProvider.UsingRuleContainerAutoInjectingEnvironmentProvider("never"),
                })
                //.UseWorlFlow(new IoCMySqlTemplateRepository(), new IoCMySqlExecutingRepository(), new Never.Serialization.EasyJsonSerializer(), (w) =>
                //{
                //    w.TemplateEngine.Register(new Template("请病假申请").Next<请假申请>().Next<组长审批, 主管审批>(CoordinationMode.Meanwhile).Next<人事审批>().End());
                //    w.TemplateEngine.Register(new Template("请节日假申请").Next<请假申请>().Next<组长审批>().Next<人事审批>().End());
                //    w.TemplateEngine.Register(new Template("请婚姻假申请").Next<请假申请>().Next<组长审批, 主管审批>().Next<人事审批>().End());
                //    w.TemplateEngine.TestCompliant += (o, e) =>
                //    {
                //        e.Register<请假申请>(new 请假申请消息(), (x, y) => { return y is 请假申请消息; });
                //        e.Register<主管审批>(new 主管审批意见结果(), (x, y) => { return y is 请假申请消息; });
                //        e.Register<组长审批>(new 组长审批意见结果(), (x, y) => { return y is 请假申请消息; });
                //        e.Register<人事审批>(new 人事审批请假意见结果(), (x, y) => { return y is 组长审批意见结果 || y is 主管审批意见结果; });
                //    };
                //})
                .UsePublishSubscribeBus()
                .UseInprocEventProviderCommandBus()
                .UseEventBus()
                //.UseHttpRequestCache()
                //.UseRabbitMQProducer(new DefaultMessageConnection() { ConnetctionString = "amqp://myRabbitMQ:myRabbitMQ@192.168.1.108:5672/sms" })
                .UseConcurrentCache("CounterDict")
                //.UseHttpRuntimeCache("RuntimeCache")
                //.UseMemoryCache("MemoryCache")
                .UseHostingDependency(e.Collector as IServiceCollection)
                .Startup();
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return base.ConfigureServices(services);
        }
    }
}
