using Never.Attributes;
using Never.Commands;
using Never.EasySql;
using Never.EasySql.SqlClient;
using Never.EventStreams;
using Never.IoC;
using Never.IoC.Providers;
using Never.Messages;
using Never.SqlClient;
using Never.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test
{
    public class Program
    {
        #region ctor

        static Program()
        {
            var app = new Never.ApplicationStartup(new AppDomainAssemblyProvider()).RegisterAssemblyFilter("Never".CreateAssemblyFilter())
                .UseEasyIoC((x, y, z) =>
                {
                    var types = y.FindClassesOfType<IWorkStep>(z.ToArray());
                    foreach (var type in types)
                    {
                        x.RegisterType(type, type);
                    }

                    //x.RegisterType<MyEventHandler, MyEventHandler>();
                    //x.RegisterType(typeof(MyGenericType<>), typeof(MyGenericType<>), string.Empty, ComponentLifeStyle.Transient);
                })
                .UseForceCheckCommandHandlerCtor()
                .UseForceCheckEventHandlerCtor()
                .UseDataContractJson()
                .UseEasyJson()
                .UseAutoInjectingAttributeUsingIoC(new[]
                {
                    SingletonAutoInjectingEnvironmentProvider.UsingRuleContainerAutoInjectingEnvironmentProvider("never"),
                })
                .UsePublishSubscribeBus()
                .UseInprocEventProviderCommandBus<DefaultCommandContext>(EmptyEventStreamStorager.Empty)
                .UseEventBus()
                //.UseHttpRequestCache()
                //.UseRabbitMQProducer(new DefaultMessageConnection() { ConnetctionString = "amqp://myRabbitMQ:myRabbitMQ@192.168.1.108:5672/sms" })
                .UseConcurrentCache("CounterDict")
                //.UseHttpRuntimeCache("RuntimeCache")
                //.UseMemoryCache("MemoryCache")
                .Startup();
        }

        public T Resolve<T>(string key = null)
        {
            return ContainerContext.Current.ServiceLocator.Resolve<T>(key);
        }

        public IServiceLocator ServiceLocator
        {
            get
            {
                return ContainerContext.Current.ServiceLocator;
            }
        }

        public IServiceActivator ServiceActivator
        {
            get
            {
                return ContainerContext.Current.ServiceActivator;
            }
        }

        public IServiceRegister ServiceRegister
        {
            get
            {
                return ContainerContext.Current.ServiceRegister;
            }
        }

        public ITypeFinder TypeFinder
        {
            get
            {
                return ContainerContext.Current.TypeFinder;
            }
        }

        public void Release()
        {
            ContainerContext.Current?.ScopeTracker?.CleanScope();
        }

        #endregion ctor

        private static void Main(string[] args)
        {
            MyMain.Main222(args);
        }
    }
}