using Never.IoC;
using System.Reflection;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 程序集提供者
    /// </summary>
    public class FilteringAssemblyProvider : IFilteringAssemblyProvider
    {
        #region field

        /// <summary>
        /// 所有程序集
        /// </summary>
        private Assembly[] assemblies = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringAssemblyProvider"/> class.
        /// </summary>
        /// <param name="assemblies">程序集.</param>
        public FilteringAssemblyProvider(Assembly[] assemblies)
        {
            this.assemblies = assemblies ?? new Assembly[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringAssemblyProvider"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public FilteringAssemblyProvider(IAssemblyProvider provider)
        {
            this.assemblies = provider == null ? new Assembly[] { } : provider.GetAssemblies();
        }
        #endregion ctor

        #region IAssemblyProvider 成员

        /// <summary>
        /// 获取所有程序集对象
        /// </summary>
        /// <returns></returns>
        public virtual Assembly[] GetAssemblies()
        {
            return this.assemblies;
        }

        #endregion IAssemblyProvider 成员

        #region replace

        /// <summary>
        /// 替换所有程序集
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        internal void ReplaceAssemblies(Assembly[] assemblies)
        {
            this.assemblies = assemblies ?? new Assembly[] { };
        }

        #endregion replace
    }
}