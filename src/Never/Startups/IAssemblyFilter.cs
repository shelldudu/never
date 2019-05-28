namespace Never.Startups
{
    /// <summary>
    /// 程序集过滤器
    /// </summary>
    public interface IAssemblyFilter
    {
        /// <summary>
        /// 要分析的程序集，为true的时候则包含分析，即在IAssemblyProcessor中出现
        /// </summary>
        /// <param name="assemblyFullName">程序集名称</param>
        /// <returns></returns>
        bool Include(string assemblyFullName);
    }
}