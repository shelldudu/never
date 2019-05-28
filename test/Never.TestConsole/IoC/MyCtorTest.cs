using Autofac;
using Never.Commands;
using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using Never.Messages;
using Never.Reflection;
using Never.Serialization;
using Never.Startups.Impls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.TestConsole.IoC
{
    public class MyCtorTest : Program
    {
        public class A
        {
            public A()
            {
            }

            public void Console2()
            {
                Console.WriteLine("ok");
            }
        }

        public class B
        {
            public B(D d)
            {
                Console.WriteLine(d.GetHashCode());
            }
        }

        public class C
        {
            public C(D d)
            {
                Console.WriteLine(d.GetHashCode());
            }
        }

        public class D
        {
        }

        [Xunit.Fact]
        public void TestCtor()
        {
            //var mygen = this.Resolve<MyGenericType<int>>();
            //var mo = this.Resolve<IMock>();
            //mo.CallBack += (p) => { Console.WriteLine("ppp"); };
            //mo.Write(new Program());

            var json = this.Resolve<IJsonSerializer>();
            //var json2 = this.Resolve<IJsonSerializer>(StaticConfiguration.SerializerKey.EasyJson);
            this.Release();
        }

        /// <summary>
        /// Tests the resolve.
        /// </summary>
        [Xunit.Fact]
        public void TestResolve()
        {
            try
            {
                this.ServiceRegister.RegisterType(typeof(Registory), typeof(IRegistory<int>));
                this.ServiceRegister.RegisterType(typeof(Registory), typeof(IJsonSerializer), "", ComponentLifeStyle.Transient);
                //this.ServiceRegister.RegisterType<MyCtorTest.A, MyCtorTest.A>(string.Empty, ComponentLifeStyle.Singleton);
                //this.ServiceRegister.RegisterType<MyCtorTest.A, MyCtorTest.A>(string.Empty, ComponentLifeStyle.Singleton);
                var ser = this.ServiceLocator.Resolve<A>("ET");
                Console.WriteLine(ser.GetHashCode());

                this.ServiceLocator.Resolve<A>();
                Console.WriteLine(ser.GetHashCode());

                this.Release();

                ser = this.ServiceLocator.Resolve<A>();
                Console.WriteLine(ser.GetHashCode());
                //Task.Factory.StartNew(() =>
                //{
                //    //System.Threading.Thread.Sleep(100);
                //    //ser.Console2();
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                this.Release();
            }
        }

        [Xunit.Fact]
        public void TestMethodTick()
        {
            try
            {
                int time = 1;

                var builder = new Autofac.ContainerBuilder();

                builder.RegisterType(typeof(DefaultCommandContext)).As(typeof(ICommandContext)).SingleInstance();
                builder.RegisterType(typeof(DefaultEventContext)).As(typeof(IEventContext)).SingleInstance();
                builder.RegisterType(typeof(InprocEventProviderCommandBus)).As(typeof(ICommandBus)).SingleInstance();
                builder.RegisterType(typeof(DefaultCommandContext)).As(typeof(ICommandContext)).SingleInstance();
                builder.RegisterType(typeof(EmptyCommandStreamStorager)).As(typeof(ICommandStreamStorager)).SingleInstance();
                builder.RegisterInstance(new MQEventStorager((IMessageProducer)null)).As<IEventStorager>().SingleInstance();

                builder.RegisterType(typeof(EmptyServiceLocator)).As<IServiceLocator>().SingleInstance();
                builder.RegisterType(typeof(EmptyServiceRegister)).As<IServiceRegister>().SingleInstance();
                //this.ServiceRegister.RegisterType<MyJsonSer, IJsonSerializer>(string.Empty, Never.IoC.ComponentLifeStyle.Singleton);
                Console.WriteLine(string.Format("进行{0}次计算，目标是构建ICommandBus", time.ToString("N2")));
                using (var container = builder.Build())
                {
                    using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
                    {
                        for (var i = 0; i < time; i++)
                        {
                            var res = container.Resolve<ICommandBus>();
                        }
                    }
                }

                using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
                {
                    for (var i = 0; i < time; i++)
                    {
                        var res = this.Resolve<ICommandBus>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                this.Release();
            }
        }

        [Xunit.Fact]
        public void TeseGenericType()
        {
            this.ServiceRegister.RegisterType<Program1, Program>();
            this.ServiceRegister.RegisterType<Program2, Program>();
            var a = this.ServiceLocator.Resolve<Program[]>();
            var b = this.ServiceLocator.ResolveOptional<IEnumerable<IEnumerable<Program>>>();
            var ba = this.ServiceLocator.ResolveOptional<IEnumerable<Program>[]>();
            //var serviceType = typeof(IRegistory<>);
            //var implServiceType = typeof(Registory<>);
            //var genericResult = serviceType.IsGenericType | implServiceType.IsGenericType;
            //var genericResult = serviceType.IsGenericTypeDefinition & implServiceType.IsGenericTypeDefinition;

            //genericResult = TypeHelper.IsContainType(implServiceType, serviceType);
        }

        public class Program1 : Program { }

        public class Program2 : Program { }

        [Xunit.Fact]
        public void TestWithAutofac()
        {
            var builder = new Autofac.ContainerBuilder();
            builder.RegisterType<Foobar>().As<IFoobar>().InstancePerLifetimeScope();
            builder.RegisterType<PingPing>().As<IPingPing>().InstancePerLifetimeScope();
            var container = builder.Build();

            var scope = container.BeginLifetimeScope();
            var f1 = scope.Resolve<IFoobar>();
            var f2 = scope.Resolve<IFoobar>();
            Console.WriteLine(f1 == f2);

            f2 = scope.BeginLifetimeScope().Resolve<IFoobar>();
            Console.WriteLine(f1 == f2);
        }

        [Xunit.Fact]
        public void TestWithEasyIoC()
        {
            var builder = ContainerContext.Current.ServiceRegister;
            builder.RegisterType<Foobar, IFoobar>(string.Empty, ComponentLifeStyle.Scoped);
            builder.RegisterType<PingPing, IPingPing>(string.Empty, ComponentLifeStyle.Scoped);
            var container = ContainerContext.Current.ServiceLocator;

            var scope = container.BeginLifetimeScope();
            var f1 = scope.Resolve<IFoobar>();
            var f2 = scope.Resolve<IFoobar>();
            Console.WriteLine(f1 == f2);

            using (var s = scope.BeginLifetimeScope())
            {
                f2 = s.Resolve<IFoobar>();
            }

            Console.WriteLine(f1 == f2);
        }

        [SingletonAutoInjectingAttribute("test", typeof(Never.Serialization.IJsonSerializer), GlobalConstantSetting.SerializerKey.EasyJson)]
        [ScopedAutoInjecting("test", typeof(Never.Serialization.IJsonSerializer))]
        public class MyJsonSer : Never.Serialization.IJsonSerializer
        {
            public MyJsonSer(IJsonSerializer ser, IRegistory<int> repi)
            {
            }

            public object DeserializeObject(string json, Type targetType)
            {
                throw new NotImplementedException();
            }

            public T Deserialize<T>(string json)
            {
                throw new NotImplementedException();
            }

            public string SerializeObject(object @object)
            {
                throw new NotImplementedException();
            }

            public string Serialize<T>(T @object)
            {
                throw new NotImplementedException();
            }
        }

        public interface IRegistory<T>
        {
        }

        [SingletonAutoInjectingAttribute("test", typeof(IRegistory<int>))]
        [SingletonAutoInjectingAttribute("test", typeof(IJsonSerializer))]
        public class Registory : IRegistory<int>, IJsonSerializer
        {
            public object DeserializeObject(string json, Type targetType)
            {
                throw new NotImplementedException();
            }

            public T1 Deserialize<T1>(string json)
            {
                throw new NotImplementedException();
            }

            public string SerializeObject(object @object)
            {
                throw new NotImplementedException();
            }

            public string Serialize<T1>(T1 @object)
            {
                throw new NotImplementedException();
            }
        }

        [Xunit.Fact]
        public void TestResolveAAA()
        {
            var register = this.ServiceRegister as Never.IoC.Injections.ServiceRegister;
            register.RegisterType<BasicBBB, BasicBBB>();
            register.RegisterType<BasicCCC, IRegistory<int>>();
            register.RegisterType<BasicDDD, IRegistory<int>>("ddd", ComponentLifeStyle.Transient);
            register.RegisterType<BasicAAA, BasicAAA>(string.Empty, ComponentLifeStyle.Singleton);

            var t = this.ServiceLocator.ResolveOptional<BasicAAA>();
        }

        public class BasicAAA
        {
            public BasicAAA(BasicBBB bbb, IRegistory<int> ccc)
            {
            }
        }

        public class BasicBBB
        {
        }

        public class BasicCCC : IRegistory<int>
        {
        }

        public class BasicDDD : IRegistory<int>
        {
        }

        public interface IFoobar : IDisposable
        {
        }

        public interface IPingPing
        {
        }

        public class Foobar : IFoobar
        {
            ~Foobar()
            {
                Console.WriteLine("Foobar.Finalize()");
            }

            public void Dispose()
            {
                Console.WriteLine("Foobar.Dispose()");
            }
        }

        public class PingPing : IPingPing
        {
            ~PingPing()
            {
                Console.WriteLine("PingPing.Finalize()");
            }

            public void Dispose()
            {
                Console.WriteLine("PingPing.Dispose()");
            }
        }

        public void TestFoobar()
        {
            var container = new EasyContainer();
            container.Init();
            container.ServiceRegister.RegisterType(typeof(Foobar), typeof(IFoobar), string.Empty, ComponentLifeStyle.Scoped);
            container.Startup();
            //container.ServiceLocator.Resolve<IFoobar>().Dispose();
            var life = container.ServiceLocator.Resolve<Never.IoC.ILifetimeScope>();
            GC.Collect();

            var scope = container.ServiceLocator.BeginLifetimeScope();
            var scope1 = scope.BeginLifetimeScope();
            var scope2 = scope.BeginLifetimeScope();

            Console.WriteLine(ReferenceEquals(scope.Resolve<IFoobar>(), scope1.Resolve<IFoobar>()));
            Console.WriteLine(ReferenceEquals(scope1.Resolve<IFoobar>(), scope1.Resolve<IFoobar>()));
            Console.WriteLine(ReferenceEquals(scope1.Resolve<IFoobar>(), scope2.Resolve<IFoobar>()));
        }
    }
}