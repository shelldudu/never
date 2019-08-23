using Never.Commands;
using Never.EventStreams;
using Never.IoC;
using Never.IoC.Providers;
using System.Collections.Generic;
using static Never.Test.MyCtorTest;

namespace Never.Test
{
    public class Program
    {
        #region ctor

        static Program()
        {
            var app = new Never.ApplicationStartup(new AppDomainAssemblyProvider()).RegisterAssemblyFilter("Never".CreateAssemblyFilter())
                .UseEasyIoC((x, y, z) =>
                {
                    x.RegisterType<MMMTTT, MMMTTT>();
                    x.RegisterType<TTTMMM, TTTMMM>();
                    x.RegisterType(typeof(GRegistory<>), typeof(IRegistory<>), string.Empty, ComponentLifeStyle.Scoped);
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
                .Startup(x =>
                {
                    using (var sc = x.ServiceLocator.BeginLifetimeScope())
                    {
                        var a = sc.Resolve<MMMTTT>();
                        var b = sc.Resolve<IRegistory<int>>();
                        System.Console.WriteLine(b.GetHashCode());
                    }

                    using (var sc = x.ServiceLocator.BeginLifetimeScope())
                    {
                        var a = sc.Resolve<TTTMMM>();
                        var b = sc.Resolve<IRegistory<int>>();
                        System.Console.WriteLine(b.GetHashCode());
                    }
                });
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
            var p = new Program();
            p.Release();
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