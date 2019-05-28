using System.Reflection;

namespace Never.Startups
{
    /// <summary>
    /// 提供已过滤程序集提供者
    /// </summary>
    public interface IFilteringAssemblyProvider
    {
        /// <summary>
        /// 获取所有过滤好的程序集对象
        /// </summary>
        /// <returns></returns>
        Assembly[] GetAssemblies();
    }
}