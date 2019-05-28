using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 程序集提供者
    /// </summary>
    public interface IAssemblyProvider
    {
        /// <summary>
        /// 获取所有程序集对象
        /// </summary>
        /// <returns></returns>
        Assembly[] GetAssemblies();
    }
}