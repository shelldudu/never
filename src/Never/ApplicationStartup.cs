using Never.Attributes;
using Never.Exceptions;
using Never.IoC;
using Never.IoC.Injections;
using Never.IoC.Providers;
using Never.Logging;
using Never.Startups;
using Never.Startups.Impls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Never
{
    /// <summary>
    /// 程序宿主环境配置服务
    /// </summary>
    public class ApplicationStartup : IApplicationStartup, IDisposable
    {
        #region field

        /// <summary>
        /// 启动队列
        /// </summary>
        private readonly List<IStartupService> startServices = new List<IStartupService>(50);

        /// <summary>
        /// 启动队列
        /// </summary>
        private readonly List<IStartupService> lastStartServices = new List<IStartupService>(2);

        /// <summary>
        /// 过滤程序集队列
        /// </summary>
        private readonly List<IAssemblyFilter> assemblyFilters = new List<IAssemblyFilter>(50);

        /// <summary>
        /// Ioc容器
        /// </summary>
        private readonly IContainer container = null;

        /// <summary>
        /// 程序集提供者
        /// </summary>
        private readonly IAssemblyProvider assemblyProvider = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStartup"/> class.
        /// </summary>
        public ApplicationStartup() : this(DefaultAssemblyProvider.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStartup"/> class.
        /// </summary>
        /// <param name="assemblyProvider">程序集提供者</param>
        public ApplicationStartup(IAssemblyProvider assemblyProvider) : this(assemblyProvider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStartup"/> class.
        /// </summary>
        /// <param name="assemblyProvider">程序集提供者</param>
        /// <param name="containerInit">如何初始化容器</param>
        protected ApplicationStartup(IAssemblyProvider assemblyProvider, Func<Func<IFilteringAssemblyProvider>, IContainer> containerInit)
        {
            this.assemblyProvider = assemblyProvider;

            /*使用简单的IoC服务，是因为使用者可能在未使用其他IoC前使用了IoC特性行为，造成出错，而且在
            简单的Service中，简单IoC对象服务已经可以满足性能要求*/

            this.container = containerInit == null ? new EasyContainer(() => { return this.FilteringAssemblyProvider; }) : containerInit(() => { return this.FilteringAssemblyProvider; });
            this.ServiceRegister = this.container.ServiceRegister;
            this.TypeFinder = this.container.TypeFinder;

            /*因为服务定位是有线程策略的，所以不适宜在初始化的时候构建该对象*/
            this.Items = new Dictionary<string, object>(20);
        }

        #endregion ctor

        #region peoperty

        /// <summary>
        /// 上下文
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// 服务定位器，只有注册了ReplaceServiceLocator后对象不为空
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// 服务注册器，只有注册了ReplaceServiceRegister后对象不为空
        /// </summary>
        public IServiceRegister ServiceRegister { get; private set; }

        /// <summary>
        /// 是否启动了
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// 类型查找
        /// </summary>
        public ITypeFinder TypeFinder { get; private set; }

        /// <summary>
        /// 过滤的程序集提供者，只有Start后才不会为空
        /// </summary>
        public IFilteringAssemblyProvider FilteringAssemblyProvider { get; private set; }

        /// <summary>
        /// 当前是否为web host环境
        /// </summary>
        public virtual bool IsWebHosted
        {
            get { return false; }
        }

        #endregion peoperty

        #region 替换

        /// <summary>
        /// 替换注册服务器
        /// </summary>
        /// <param name="serviceRegister">服务注册器</param>
        /// <returns></returns>
        [NotNull(Name = "serviceRegister")]
        public ApplicationStartup ReplaceServiceRegister(IServiceRegister serviceRegister)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法替换");

            this.ServiceRegister = serviceRegister;
            return this;
        }

        /// <summary>
        /// 替换服务定位器
        /// </summary>
        /// <param name="serviceLocator">服务定位器</param>
        /// <returns></returns>
        [NotNull(Name = "serviceLocator")]
        public ApplicationStartup ReplaceServiceLocator(IServiceLocator serviceLocator)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法替换");

            this.ServiceLocator = serviceLocator;
            return this;
        }

        /// <summary>
        /// 替换类型发现者
        /// </summary>
        /// <param name="typeFinder">类型发现者</param>
        /// <returns></returns>
        [NotNull(Name = "typeFinder")]
        public ApplicationStartup ReplaceTypeFinder(ITypeFinder typeFinder)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法替换");

            this.TypeFinder = typeFinder;
            return this;
        }

        #endregion 替换

        #region 注册

        /// <summary>
        /// 新增过滤类型
        /// </summary>
        /// <param name="assemblyFilter">程序集过滤器</param>
        [NotNull(Name = "typeFiltering")]
        public ApplicationStartup RegisterAssemblyFilter(IAssemblyFilter assemblyFilter)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            if (assemblyFilter != null)
                this.assemblyFilters.Add(assemblyFilter);

            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务，服务会按order排序
        /// </summary>
        /// <param name="order">排序</param>
        /// <param name="onStarting">启动服务</param>
        /// <returns></returns>
        public ApplicationStartup RegisterStartService(int order, Action<StartupContext> onStarting)
        {
            return this.RegisterStartService(new MyStartService() { order = order, onStarting = onStarting });
        }

        /// <summary>
        /// 新增初始化上下文服务，服务会按order排序
        /// </summary>
        /// <param name="startService">启动服务</param>
        /// <returns></returns>
        public ApplicationStartup RegisterStartService(IStartupService startService)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            if (startService != null)
                this.startServices.Add(startService);

            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务，服务会按order排序，如果是再后一个的，则按加入顺序执行
        /// </summary>
        /// <param name="last">是否放在最后一个服务中</param>
        /// <param name="onStarting">启动服务</param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ApplicationStartup RegisterStartService(bool last, Action<StartupContext> onStarting)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            if (!last)
                return this.RegisterStartService(new MyStartService() { order = int.MaxValue, onStarting = onStarting });

            this.lastStartServices.Add(new MyStartService() { order = 0, onStarting = onStarting });
            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务，服务会按order排序，如果是再后一个的，则按加入顺序执行
        /// </summary>
        /// <param name="startService">启动服务</param>
        /// <param name="last">是否放在最后一个服务中</param>
        /// <returns></returns>
        public ApplicationStartup RegisterStartService(bool last, IStartupService startService)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            if (!last)
                return this.RegisterStartService(startService);

            if (startService != null)
                this.lastStartServices.Add(startService);

            return this;
        }

        /// <summary>
        /// 增初始化上下文服务，该方法会在最后执行，按加入顺序执行
        /// </summary>
        /// <param name="startupService">启动服务</param>
        /// <returns></returns>
        public ApplicationStartup AddIntoFinalStartService(IStartupService startupService)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            this.lastStartServices.Add(startupService);
            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务，该方法会在最后执行，按加入顺序执行
        /// </summary>
        /// <param name="onStarting">启动服务</param>
        public ApplicationStartup AddIntoFinalStartService(Action<StartupContext> onStarting)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            this.lastStartServices.Add(new MyStartService() { order = int.MaxValue, onStarting = onStarting });
            return this;
        }

        /// <summary>
        /// 增初始化上下文服务，该方法会在最后执行，并且插入第一个执行位置
        /// </summary>
        /// <param name="startupService">启动服务</param>
        /// <returns></returns>
        public ApplicationStartup InsertIntoFinalStartService(IStartupService startupService)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            this.lastStartServices.Insert(0, startupService);
            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务，该方法会在最后执行，并且插入第一个执行位置
        /// </summary>
        /// <param name="onStarting">启动服务</param>
        public ApplicationStartup InsertIntoFinalStartService(Action<StartupContext> onStarting)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法注册");

            this.lastStartServices.Insert(0, new MyStartService() { order = int.MaxValue, onStarting = onStarting });
            return this;
        }

        /// <summary>
        /// 新增初始化上下文服务
        /// </summary>
        /// <param name="startService">启动服务</param>
        /// <returns></returns>
        public ApplicationStartup RemoveStartService(IStartupService startService)
        {
            if (this.IsStarted)
                throw new FriendlyException("程序已启动，无法删除");

            if (startService != null)
            {
                this.startServices.Remove(startService);
                this.lastStartServices.Remove(startService);
            }

            return this;
        }

        #endregion 注册

        #region IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposed"></param>
        protected virtual void Dispose(bool disposed)
        {
            this.startServices.Clear();
            this.assemblyFilters.Clear();
        }

        #endregion IDisposable

        #region start

        private class MyStartService : IStartupService
        {
            public Action<StartupContext> onStarting = null;
            public int order = 0;

            int IStartupService.Order
            {
                get
                {
                    return this.order;
                }
            }

            void IStartupService.OnStarting(StartupContext context)
            {
                if (this.onStarting != null)
                    this.onStarting.Invoke(context);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public ApplicationStartup Startup()
        {
            return this.Startup(null);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="action">操作</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ApplicationStartup Startup(Action<ApplicationStartup> action)
        {
            return this.Startup(TimeSpan.Zero, action);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sleepTime"></param>
        /// <param name="action">操作</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ApplicationStartup Startup(TimeSpan sleepTime, Action<ApplicationStartup> action)
        {
            if (this.IsStarted)
                return this;

            if (sleepTime > TimeSpan.Zero)
            {
                Console.WriteLine("startup is sleep now");
                System.Threading.Thread.Sleep(sleepTime);
                Console.WriteLine("startup has been pulse");
            }

            this.IsStarted = true;

            /*得到过滤的程序集*/
            this.InitFilteringAssemblyProvider();

            /*注入*/
            this.ServiceRegister.RegisterInstance(this.assemblyProvider, typeof(IAssemblyProvider), string.Empty);
            /*注入*/
            this.ServiceRegister.RegisterInstance(this.TypeFinder, typeof(ITypeFinder), string.Empty);

            /*开启服务*/
            this.StartService(new StartupContext(this) { Container = ContainerContext.Current ?? this.container });

            if (this.assemblyFilters != null)
                this.assemblyFilters.Clear();

            if (this.startServices != null)
                this.startServices.Clear();

            if (this.lastStartServices != null)
                this.lastStartServices.Clear();

            if (action != null)
                action.Invoke(this);

            return this;
        }

        /// <summary>
        /// 得到过滤的程序集提供器
        /// </summary>
        private void InitFilteringAssemblyProvider()
        {
            if (this.FilteringAssemblyProvider != null)
                return;

            var assemblies = this.assemblyProvider.GetAssemblies();
            if (assemblies == null)
                assemblies = new Assembly[0];

            var filters = new List<Assembly>(30);
            foreach (var i in assemblies)
            {
                if (this.FilterAny(i))
                    filters.Add(i);
            }

            /*得到过滤的程序集*/
            this.FilteringAssemblyProvider = new FilteringAssemblyProvider(filters.ToArray());
        }

        /// <summary>
        /// 过滤程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        private bool FilterAny(Assembly assembly)
        {
            if (this.assemblyFilters == null || this.assemblyFilters.Count == 0)
                return true;

            foreach (var ass in this.assemblyFilters)
            {
                if (ass.Include(assembly.FullName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="context">启动上下文</param>
        private void StartService(StartupContext context)
        {
            if (this.startServices == null)
                return;

            if (context == null)
                return;

            this.startServices.Sort(this.StartServiceComparison);
            foreach (var service in this.startServices)
            {
                if (service == null)
                    continue;

                service.OnStarting(context);
            }

            //this.lastStartServices.Sort(this.StartServiceComparison);
            foreach (var service in this.lastStartServices)
            {
                if (service == null)
                    continue;

                service.OnStarting(context);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int StartServiceComparison(IStartupService x, IStartupService y)
        {
            if (x == null || y == null)
                return -1;

            if (x.Equals(y))
                return 1;

            return x.Order > y.Order ? 1 : -1;
        }

        #endregion start

        #region ioc

        /// <summary>
        /// 使用空的loggerbuilder
        /// </summary>
        /// <returns></returns>
        public ApplicationStartup UseEmptyLogger()
        {
            this.ServiceRegister.RegisterType(typeof(LoggerBuilder), typeof(ILoggerBuilder), "empty", ComponentLifeStyle.Singleton);
            return this;
        }

        /// <summary>
        /// 使用简单Ioc
        /// </summary>
        public ApplicationStartup UseEasyIoC()
        {
            return this.UseEasyIoC(null, null);
        }

        /// <summary>
        /// 使用简单Ioc
        /// </summary>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        public ApplicationStartup UseEasyIoC(Action<RegisterRuleCollector, ITypeFinder, IEnumerable<Assembly>> onIniting)
        {
            return this.UseEasyIoC(onIniting, null);
        }

        /// <summary>
        /// 使用简单Ioc，回调的方法
        /// </summary>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        /// <param name="onStarting">在start方法后最后一个startservice执行的</param>
        public ApplicationStartup UseEasyIoC(Action<RegisterRuleCollector, ITypeFinder, IEnumerable<Assembly>> onIniting, Action<RegisterRuleCollector, ITypeFinder, IEnumerable<Assembly>> onStarting)
        {
            if (this.IsStarted)
                return this;

            var easyContainer = this.container as EasyContainer;
            if (easyContainer == null)
                return this;

            /*得到过滤的程序集*/
            this.InitFilteringAssemblyProvider();

            if (onIniting != null)
                easyContainer.OnIniting += (s, e) => { onIniting.Invoke((RegisterRuleCollector)e.Collector, e.TypeFinder, e.Assemblies); };

            easyContainer.Init();

            this.InsertIntoFinalStartService(x => 
            {
                if (onStarting != null)
                    easyContainer.OnStarting += (s, e) => { onStarting.Invoke((RegisterRuleCollector)e.Collector, e.TypeFinder, e.Assemblies); };

                easyContainer.Startup();
                this.ServiceLocator = easyContainer.ServiceLocator;
            });

            this.ServiceRegister = easyContainer.ServiceRegister;
            ContainerContext.Current = easyContainer;
            return this;
        }

        #endregion ioc
    }
}