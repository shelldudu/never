using System;
using System.Collections.Generic;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 没有任何服务的程序宿主环境配置服务
    /// </summary>
    public sealed class EmptyApplicationStartup : IApplicationStartup, IDisposable
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyApplicationStartup Only
        {
            get
            {
                if (Singleton<EmptyApplicationStartup>.Instance == null)
                    Singleton<EmptyApplicationStartup>.Instance = new EmptyApplicationStartup();

                return Singleton<EmptyApplicationStartup>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        private EmptyApplicationStartup()
        {
        }

        #endregion ctor

        #region IApplicationStartup

        /// <summary>
        /// 服务定位器
        /// </summary>
        IoC.IServiceLocator IApplicationStartup.ServiceLocator
        {
            get { return EmptyServiceLocator.Only; }
        }

        /// <summary>
        /// 服务注册器
        /// </summary>
        IoC.IServiceRegister IApplicationStartup.ServiceRegister
        {
            get { return EmptyServiceRegister.Only; }
        }

        /// <summary>
        /// 类型查找
        /// </summary>
        IoC.ITypeFinder IApplicationStartup.TypeFinder
        {
            get { return EmptyTypeFinder.Only; }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// 提供已过滤程序集提供者
        /// </summary>
        IFilteringAssemblyProvider IApplicationStartup.FilteringAssemblyProvider
        {
            get { return EmptyFilteringAssemblyProvider.Only; }
        }

        /// <summary>
        /// 上下文
        /// </summary>
        IDictionary<string, object> IApplicationStartup.Items
        {
            get { return new Dictionary<string, object>(); }
        }

        /// <summary>
        /// 当前是否为web host环境
        /// </summary>
        bool IApplicationStartup.IsWebHosted
        {
            get { return false; }
        }

        #endregion IApplicationStartup
    }
}