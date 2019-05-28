using Never.IoC;
using Never.Startups.Impls;
using System.Collections.Generic;

namespace Never.Startups
{
    /// <summary>
    /// 启动上下文
    /// </summary>
    public class StartupContext
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        private static StartupContext empty = new StartupContext(EmptyApplicationStartup.Only);

        /// <summary>
        /// 程序宿主环境配置
        /// </summary>
        public readonly IApplicationStartup ApplicationStartup = null;

        #endregion field

        #region property

        /// <summary>
        /// 提供当前过滤的所有程序集
        /// </summary>
        public IFilteringAssemblyProvider FilteringAssemblyProvider
        {
            get { return this.ApplicationStartup.FilteringAssemblyProvider; }
        }

        /// <summary>
        /// 类型查找
        /// </summary>
        public ITypeFinder TypeFinder
        {
            get { return this.ApplicationStartup.TypeFinder; }
        }

        /// <summary>
        /// 服务定位器
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get { return this.ApplicationStartup.ServiceLocator; }
        }

        /// <summary>
        /// 容器上下文
        /// </summary>
        public IContainer Container
        {
            get; internal set;
        }

        /// <summary>
        /// 服务注册器
        /// </summary>
        public IServiceRegister ServiceRegister
        {
            get { return this.ApplicationStartup.ServiceRegister; }
        }

        /// <summary>
        /// 上下文字典
        /// </summary>
        public IDictionary<string, object> Items { get; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupContext"/> class.
        /// </summary>
        /// <param name="applicationStartup">程序宿主环境配置.</param>
        public StartupContext(IApplicationStartup applicationStartup)
        {
            this.ApplicationStartup = applicationStartup ?? EmptyApplicationStartup.Only;
            this.Items = (applicationStartup ?? EmptyApplicationStartup.Only).Items;
            if (this.Items == null)
                this.Items = new Dictionary<string, object>(20);
        }

        #endregion ctor

        #region empty

        /// <summary>
        /// 空对象
        /// </summary>
        public static StartupContext Empty
        {
            get
            {
                return empty;
            }
        }

        #endregion empty

        #region 处理

        /// <summary>
        /// 处理每种类型，在其内部遍历所有加载程序集里面的对象类型
        /// </summary>
        /// <param name="typeProcessor">对象类型加工处理器</param>
        public void ProcessType(ITypeProcessor typeProcessor)
        {
            if (typeProcessor == null)
                return;

            this.ProcessType(new[] { typeProcessor });
        }

        /// <summary>
        /// 处理每种类型，在其内部遍历所有加载程序集里面的对象类型
        /// </summary>
        /// <param name="typeProcessors">对象类型加工处理器</param>
        public void ProcessType(IEnumerable<ITypeProcessor> typeProcessors)
        {
            if (typeProcessors == null)
                return;

            foreach (var assembly in this.FilteringAssemblyProvider.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var processer in typeProcessors)
                    {
                        if (processer == null)
                            continue;

                        processer.Processing(this.ApplicationStartup, type);
                    }
                }
            }
        }

        /// <summary>
        /// 处理每种类型，在其内部遍历所有加载程序集类型
        /// </summary>
        /// <param name="assemblyProcessor">程序集加工处理器</param>
        public void ProcessAssembly(IAssemblyProcessor assemblyProcessor)
        {
            if (assemblyProcessor == null)
                return;

            this.ProcessAssembly(new[] { assemblyProcessor });
        }

        /// <summary>
        /// 处理每种类型，在其内部遍历所有加载程序集类型
        /// </summary>
        /// <param name="assemblyProcessors">程序集加工处理器</param>
        public void ProcessAssembly(IEnumerable<IAssemblyProcessor> assemblyProcessors)
        {
            if (assemblyProcessors == null)
                return;

            foreach (var assembly in this.FilteringAssemblyProvider.GetAssemblies())
            {
                foreach (var processor in assemblyProcessors)
                {
                    if (processor == null)
                        continue;

                    processor.Processing(this.ApplicationStartup, assembly);
                }
            }
        }

        #endregion 处理
    }
}