using Never.IoC.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    /// 组件生命范围定义
    /// </summary>
    public interface ILifetimeScope : IDisposable
    {
        /// <summary>
        /// 开启范围
        /// </summary>
        /// <returns></returns>
        ILifetimeScope BeginLifetimeScope();

        /// <summary>
        /// 构造所有对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object[] ResolveAll(Type serviceType);

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object Resolve(Type serviceType, string key);

        /// <summary>
        /// 优化构造对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object ResolveOptional(Type serviceType);

        /// <summary>
        /// 在释放的过程中
        /// </summary>
        event EventHandler OnDisposed;
    }
}