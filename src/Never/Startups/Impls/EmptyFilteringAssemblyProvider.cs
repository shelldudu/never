namespace Never.Startups.Impls
{
    /// <summary>
    /// 空的已过滤程序集提供者
    /// </summary>
    public sealed class EmptyFilteringAssemblyProvider : IFilteringAssemblyProvider
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyFilteringAssemblyProvider Only
        {
            get
            {
                if (Singleton<EmptyFilteringAssemblyProvider>.Instance == null)
                    Singleton<EmptyFilteringAssemblyProvider>.Instance = new EmptyFilteringAssemblyProvider();

                return Singleton<EmptyFilteringAssemblyProvider>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyFilteringAssemblyProvider"/> class from being created.
        /// </summary>
        private EmptyFilteringAssemblyProvider()
        {
        }

        #endregion ctor

        /// <summary>
        /// 获取所有过滤好的程序集对象
        /// </summary>
        /// <returns></returns>
        System.Reflection.Assembly[] IFilteringAssemblyProvider.GetAssemblies()
        {
            return new System.Reflection.Assembly[] { };
        }
    }
}