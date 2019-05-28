using System.Reflection;

namespace Never.Startups
{
    /// <summary>
    /// 程序集加工处理器
    /// </summary>
    public interface IAssemblyProcessor
    {
        /// <summary>
        /// 处理程序集
        /// </summary>
        /// <param name="application">程序宿主环境配置</param>
        /// <param name="assembly">程序集</param>
        void Processing(IApplicationStartup application, Assembly assembly);
    }
}