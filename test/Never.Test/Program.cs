using Never.Commands;
using Never.DataAnnotations;
using Never.EventStreams;
using Never.IoC;
using Never.IoC.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
                        var ser = sc.Resolve<Never.Serialization.IJsonSerializer>();
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
            // System.IO.File.AppendAllLines("d:\\tutui.txt",System.IO.File.ReadAllLines("d:\\tuitu.txt").Reverse());
            var a1 = System.IO.File.ReadAllLines(@"C:\Users\shelldudu\Documents\leidian\Pictures\a1.txt");
            var a2 = System.IO.File.ReadAllLines(@"C:\Users\shelldudu\Documents\leidian\Pictures\a2.txt");
            var list = new List<String>();
            //list.AddRange(a1);
            list.AddRange(a2);
            list.Sort();
            //list.Reverse();
            System.IO.File.WriteAllLines(@"C:\Users\shelldudu\Documents\leidian\Pictures\a2.txt", list);
        }

        private static void ChangeBuilder(System.Text.StringBuilder builder)
        {
            builder = new System.Text.StringBuilder("bbb");
        }

        private static void ChangeABC(ABC a)
        {
            var colletion = new List<int>();
            colletion.Add(3);
            colletion.Add(4);

            a.Collection = colletion;
        }

        public static IValidator InitValidator<T>() where T : struct
        {
            return (IValidator)default(T);
        }

        [Never.DataAnnotations.Validator(typeof(MyStructValidator))]
        public struct ABC : Never.DataAnnotations.IAmValidator
        {
            public int Id { get; set; }

            public ICollection<int> Collection { get; set; }

            public void ChangeId()
            {
                this.Id = 666;
            }

            public ValidationResult Validate()
            {
                return ValidationResult.Expression(this, (rules, target) =>
                {
                    if (target.Id <= 0)
                        rules.Add(new KeyValuePair<Expression<Func<ABC, object>>, string>(m => m.Id, "Id不可能小于0"));
                });
            }

            public class MyClassValidator : DataAnnotations.Validator<ABC>
            {
                public override IEnumerable<KeyValuePair<Expression<Func<ABC, object>>, string>> RuleFor(ABC target)
                {
                    if (target.Id <= 0)
                        yield return new KeyValuePair<Expression<Func<ABC, object>>, string>(m => m.Id, "Id不可能小于0");
                }
            }
            public struct MyStructValidator : DataAnnotations.IValidator
            {
                public IEnumerable<KeyValuePair<Expression<Func<ABC, object>>, string>> RuleFor(ABC target)
                {
                    if (target.Id <= 0)
                        yield return new KeyValuePair<Expression<Func<ABC, object>>, string>(m => m.Id, "Id不可能小于0");
                }

                public ValidationResult Validate(object target)
                {
                    if (target is ABC)
                    {
                        return ValidationResult.Expression((ABC)target, this.RuleFor);
                    }

                    return ValidationResult.Success;
                }
            }
        }

        internal class TypeProcessor : Never.Startups.ITypeProcessor
        {
            #region field 

            /// <summary>
            /// All
            /// </summary>
            static readonly IDictionary<Type, Tuple<ValidatorAttribute, Func<IValidator>>> all = null;

            #endregion

            #region ctor        
            /// <summary>
            /// Initializes the <see cref="TypeProcessor"/> class.
            /// </summary>
            static TypeProcessor()
            {
                all = new Dictionary<Type, Tuple<ValidatorAttribute, Func<IValidator>>>(20);
            }

            /// <summary>
            ///
            /// </summary>
            public TypeProcessor()
            {
            }

            #endregion ctor

            #region i am validator

            private struct MyValidator : IValidator
            {
                public ValidationResult Validate(object target)
                {
                    var validator = target as IAmValidator;
                    return validator == null ? ValidationResult.Success : validator.Validate();
                }
            }

            #endregion


            #region ITypeProcessor

            /// <summary>
            ///
            /// </summary>
            /// <param name="application"></param>
            /// <param name="type"></param>
            public void Processing(IApplicationStartup application, Type type)
            {
                if (type == null)
                    return;

                var attributes = type.GetCustomAttributes(typeof(ValidatorAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    foreach (ValidatorAttribute attribute in attributes)
                    {
                        if (attribute.ValidatorType.IsAssignableFromType(typeof(IAmValidator)) == true)
                        {
                            all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => new MyValidator());
                            return;
                        }

                        if (attribute.ValidatorType.IsAssignableFromType(typeof(IValidator)) == false)
                        {
                            continue;
                        }

                        if (attribute.ValidatorType.IsValueType)
                        {
                            var emit = Never.Reflection.EasyEmitBuilder<Func<IValidator>>.NewDynamicMethod();
                            var loal = emit.DeclareLocal(attribute.ValidatorType);
                            emit.LoadLocalAddress(loal);
                            emit.InitializeObject(attribute.ValidatorType);
                            emit.LoadLocal(loal);
                            emit.Box(attribute.ValidatorType);
                            emit.CastClass(typeof(IValidator));
                            emit.Return();
                            var @delegate = emit.CreateDelegate();
                            all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, @delegate);
                            //all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => System.Activator.CreateInstance(attribute.ValidatorType) as IValidator);
                            return;
                        }

                        var ctors = attribute.ValidatorType.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var ctor = ctors.FirstOrDefault(t => t.GetParameters().Length == 0);
                        if (ctor != null)
                        {
                            var emit = Never.Reflection.EasyEmitBuilder<Func<IValidator>>.NewDynamicMethod();
                            emit.NewObject(ctor);
                            emit.CastClass(typeof(IValidator));
                            emit.Return();
                            var @delegate = emit.CreateDelegate();
                            all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, @delegate);
                            //all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => System.Activator.CreateInstance(attribute.ValidatorType) as IValidator);
                            return;
                        }

                        throw new Exception($"{((ValidatorAttribute)attributes.FirstOrDefault()).ValidatorType} must has no parameters on ctor");
                    }
                }
            }

            #endregion ITypeProcessor

            #region contains

            /// <summary>
            /// 尝试获取特性
            /// </summary>
            /// <param name="type"></param>
            /// <param name="attribute"></param>
            /// <returns></returns>
            public static bool TryGetAttribute(Type type, out ValidatorAttribute attribute)
            {
                if (all.TryGetValue(type, out var value))
                {
                    attribute = value?.Item1;
                    return true;
                }

                attribute = null;
                return false;
            }

            /// <summary>
            /// 尝试获取对象
            /// </summary>
            /// <param name="type"></param>
            /// <param name="validator"></param>
            /// <returns></returns>
            public static bool TryGetActivator(Type type, out IValidator validator)
            {
                if (all.TryGetValue(type, out var value))
                {
                    validator = value?.Item2();
                    return true;
                }

                validator = null;
                return false;
            }

            #endregion
        }
    }
}