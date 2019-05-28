using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Never.IoC.Injections;
using Never.IoC.Injections.Rules;
using Never.IoC.Providers;
using Never.Startups;
using Never.Startups.Impls;

namespace Never.IoC
{
    /// <summary>
    /// IoC容器
    /// </summary>
    public class EasyContainer : Never.IoC.IContainer, Never.IoC.IContainerStartup, IValuableOption<UnableRegisterRule>
    {
        #region field

        /// <summary>
        /// 容器
        /// </summary>
        private readonly RegisterRuleContainer ruleContainer = null;

        /// <summary>
        /// 容器
        /// </summary>
        private ILifetimeScope rootScope = null;

        /// <summary>
        /// 服务注册器
        /// </summary>
        private readonly IServiceRegister serviceRegister = null;

        /// <summary>
        /// 创建器
        /// </summary>
        private IServiceActivator serviceActivator = null;

        /// <summary>
        /// 服务定位器
        /// </summary>
        private IServiceLocator serviceLocator = null;

        /// <summary>
        /// 类型发现者
        /// </summary>
        private readonly ITypeFinder typeFinder = null;

        /// <summary>
        /// 程序集过滤
        /// </summary>
        private readonly Func<IFilteringAssemblyProvider> filteringAssemblyProviderCallback = null;

        /// <summary>
        /// 过滤的程序集
        /// </summary>
        private Assembly[] assemblies;

        /// <summary>
        /// 是否已经初始化环境了
        /// </summary>
        private bool isInitEnvironment;

        /// <summary>
        /// 是否已经启动了
        /// </summary>
        private bool isStarted;

        /// <summary>
        /// 跟踪着
        /// </summary>
        private ILifetimeScopeTracker scopeTracker = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public EasyContainer() : this(() => { return new FilteringAssemblyProvider(DefaultAssemblyProvider.Default.GetAssemblies()); })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProvider">The filtering assembly provider.</param>
        public EasyContainer(IFilteringAssemblyProvider filteringAssemblyProvider) : this(() => filteringAssemblyProvider, (x) => new RegisterRuleContainer(x))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProviderCallback">The filtering assembly provider callback.</param>
        public EasyContainer(Func<IFilteringAssemblyProvider> filteringAssemblyProviderCallback) : this(filteringAssemblyProviderCallback, (x) => new RegisterRuleContainer(x))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProviderCallback">The filtering assembly provider callback.</param>
        /// <param name="containerActivator"></param>
        protected EasyContainer(Func<IFilteringAssemblyProvider> filteringAssemblyProviderCallback, Func<IValuableOption<UnableRegisterRule>, RegisterRuleContainer> containerActivator)
        {
            this.typeFinder = new DefaultTypeFinder();
            this.ruleContainer = containerActivator == null ? new RegisterRuleContainer(this) : containerActivator(this);
            this.serviceRegister = new ServiceRegister(this.ruleContainer, this);
            this.scopeTracker = new DefaultLifetimeScopeTracker();
            this.filteringAssemblyProviderCallback = filteringAssemblyProviderCallback;
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 注册规则容器
        /// </summary>
        public RegisterRuleContainer RuleContainer
        {
            get { return this.ruleContainer; }
        }

        #endregion prop

        #region IContainer

        /// <summary>
        /// 在系统启动过路中初始化组件
        /// </summary>
        public event EventHandler<IContainerStartupEventArgs> OnIniting;

        /// <summary>
        /// 在系统启动最后的组件
        /// </summary>
        public event EventHandler<IContainerStartupEventArgs> OnStarting;

        /// <summary>
        /// 初始化
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Init()
        {
            if (this.isInitEnvironment)
                return;

            this.isInitEnvironment = true;

            var rules = new RegisterRuleCollector(5);

            //注入引擎
            rules.RegisterInstance<IContainer>(this);

            //注入容器
            rules.RegisterInstance(this.ruleContainer, typeof(IRegisterRuleContainer));
            rules.RegisterInstance(this.ruleContainer, typeof(IRegisterRuleChangeable));
            rules.RegisterInstance(this.ruleContainer, typeof(IRegisterRuleQuery));

            //注入类型查找
            rules.RegisterInstance(this.typeFinder, typeof(ITypeFinder));

            //注入注册
            rules.RegisterInstance(this.serviceRegister, typeof(IServiceRegister));

            //注入解决
            rules.RegisterCallBack<IServiceLocator>(string.Empty, ComponentLifeStyle.Singleton, () => this.serviceLocator);

            //跟踪者
            rules.RegisterCallBack<ILifetimeScopeTracker>(string.Empty, ComponentLifeStyle.Singleton, () => this.scopeTracker);

            //注入解决
            rules.RegisterType(typeof(ConstructorList<>), typeof(IEnumerable<>));
            rules.RegisterType(typeof(ConstructorDictionary<,>), typeof(IDictionary<,>));

            //更新容器
            this.ruleContainer.Update(rules);

            //获取程序集
            this.assemblies = (this.filteringAssemblyProviderCallback == null ? new Assembly[0] : this.filteringAssemblyProviderCallback.Invoke().GetAssemblies());

            rules = new RegisterRuleCollector(100);
            this.OnIniting?.Invoke(this, new IContainerStartupEventArgs(this.typeFinder, this.assemblies, rules));

            /*更新容器*/
            this.ruleContainer.Update(rules);

            return;
        }

        /// <summary>
        /// 开始启动
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Startup()
        {
            if (this.isStarted)
                return;

            if (!this.isInitEnvironment)
                throw new Exception("call init before on start");

            var rules = new RegisterRuleCollector();
            this.OnStarting?.Invoke(this, new IContainerStartupEventArgs(this.typeFinder, this.assemblies, rules));

            /*更新容器*/
            this.ruleContainer.Update(rules);

            //注入生命周期管理
            var rule = default(RegisterRule);
            this.rootScope = this.ruleContainer.Build(out rule);
            this.serviceLocator = new ServiceLocator(this.rootScope);
            this.serviceActivator = new ServiceActivator(this.rootScope, this.ruleContainer);
            rule.Builder = rule.OptionalBuilder = (r, q, l, c) => { return ((ServiceLocator)this.serviceLocator).BeginLifetimeScope(this.rootScope, this.scopeTracker); };
            this.isStarted = true;
        }

        /// <summary>
        /// 服务定位器
        /// </summary>
        public virtual IServiceLocator ServiceLocator
        {
            get
            {
                return this.serviceLocator;
            }
        }

        /// <summary>
        /// 服务注册器
        /// </summary>
        public IServiceRegister ServiceRegister
        {
            get
            {
                return this.serviceRegister;
            }
        }

        /// <summary>
        /// 类型发现者
        /// </summary>
        public ITypeFinder TypeFinder
        {
            get
            {
                return this.typeFinder;
            }
        }

        /// <summary>
        /// 对象创建者
        /// </summary>
        public IServiceActivator ServiceActivator
        {
            get
            {
                return this.serviceActivator;
            }
        }

        /// <summary>
        /// 组件生命范围定义提供者
        /// </summary>
        public ILifetimeScopeTracker ScopeTracker
        {
            get
            {
                return this.scopeTracker;
            }
            set
            {
                this.scopeTracker = value ?? this.scopeTracker;
            }
        }

        #endregion IContainer

        #region IOption<UnableRegisterRule>

        /// <summary>
        /// TOptin对象实例
        /// </summary>
        UnableRegisterRule IValuableOption<UnableRegisterRule>.Value => new UnableRegisterRule(this.rootScope != null);

        #endregion IOption<UnableRegisterRule>
    }
}