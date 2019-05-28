using Never.Attributes;
using System.Text.RegularExpressions;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 正则过滤器
    /// </summary>
    public class RegexAssemblyFilter : IAssemblyFilter
    {
        #region field

        /// <summary>
        /// 程序集过滤的正则
        /// </summary>
        private readonly Regex assemblyRegex = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexAssemblyFilter"/> class.
        /// </summary>
        /// <param name="assemblyRegex">过滤的正则.</param>
        public RegexAssemblyFilter(Regex assemblyRegex)
        {
            this.assemblyRegex = assemblyRegex;
        }

        #endregion ctor

        #region ITypeFiltering成员

        /// <summary>
        /// 根据正则过滤程序集
        /// </summary>
        /// <param name="assemblyFullName">程序集名称</param>
        /// <returns></returns>
        [NotNull(Name = "assembly")]
        public bool Include(string assemblyFullName)
        {
            return this.assemblyRegex != null && this.assemblyRegex.IsMatch(assemblyFullName) && !this.Exclude(assemblyFullName);
        }

        /// <summary>
        /// 排除
        /// </summary>
        /// <param name="assemblyFullName">程序集名称</param>
        /// <returns></returns>
        protected virtual bool Exclude(string assemblyFullName)
        {
            return false;
        }

        #endregion ITypeFiltering成员
    }
}