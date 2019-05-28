using Never.Caching;
using Never.IoC;
using Never.IoC.Providers;
using Never.Startups;
using Never.Startups.Impls;
using Never.Web.IoC.Providers;
using System;

namespace Never.Web.IoC
{
    /// <summary>
    /// Web环境下的IoC容器
    /// </summary>
    public class WebEasyContainer : EasyContainer
    {
        #region ctor

#if !NET461
        /// <summary>
        ///
        /// </summary>
        public WebEasyContainer() : base(() => { return new FilteringAssemblyProvider(WebDomainAssemblyProvider.Default.GetAssemblies()); })
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProvider">The filtering assembly provider.</param>
        public WebEasyContainer(IFilteringAssemblyProvider filteringAssemblyProvider) : base(filteringAssemblyProvider)
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProviderCallback">The filtering assembly provider callback.</param>
        public WebEasyContainer(Func<IFilteringAssemblyProvider> filteringAssemblyProviderCallback) : base(filteringAssemblyProviderCallback)
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }
#else

        /// <summary>
        ///
        /// </summary>
        public WebEasyContainer() : base(() => { return new FilteringAssemblyProvider(WebDomainAssemblyProvider.Default.GetAssemblies()); })
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProvider">The filtering assembly provider.</param>
        public WebEasyContainer(IFilteringAssemblyProvider filteringAssemblyProvider) : base(filteringAssemblyProvider)
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyContainer"/> class.
        /// </summary>
        /// <param name="filteringAssemblyProviderCallback">The filtering assembly provider callback.</param>
        public WebEasyContainer(Func<IFilteringAssemblyProvider> filteringAssemblyProviderCallback) : base(filteringAssemblyProviderCallback)
        {
            this.ScopeTracker = new WebLifetimeScopeTracker();
        }

#endif

        #endregion ctor
    }
}