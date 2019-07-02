using System;
using System.Collections.Generic;
using Never.Commands;
using Never.EventStreams;
using Never.IoC;
using Never.IoC.Providers;
using Never.TestConsole.Caching;
using Never.TestConsole.Performance;
using Never.TestConsole.SqlClient;
using Never.Utils;

namespace Never.TestConsole
{
    public class Program
    {
        #region ctor

        static Program()
        {
            var app = new Never.ApplicationStartup(new AppDomainAssemblyProvider()).RegisterAssemblyFilter("Never".CreateAssemblyFilter())
                .UseEasyIoC((x, y, z) =>
                {
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
            ContainerContext.Current.ServiceLocator.ScopeTracker?.CleanScope();
        }

        #endregion ctor

        private static void Main(string[] args)
        {
            //var baidu = new Never.Web.HttpRequestDownloader().GetString("http://baidu.com", null, null, null, 0, out var status);
            // baidu = new Never.Web.HttpRequestDownloader().GetStringAsync("http://baidu.com").GetAwaiter().GetResult();
            // var bingo = new Never.Web.HttpClientDownloader().GetAsync("http://bing.com").GetAwaiter().GetResult();

            var a = new ABC();
            a.Collection = new List<int>();
            a.Collection.Add(2);

            ChangeABC(a);
        }

        private static void ChangeABC(ABC a)
        {
            var colletion = new List<int>();
            colletion.Add(3);
            colletion.Add(4);

            a.Collection = colletion;
        }

        public struct ABC
        {
            public int Id { get; set; }

            public ICollection<int> Collection { get; set; }

            public void ChangeId()
            {
                this.Id = 666;
            }
        }
    }
}