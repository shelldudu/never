using System;

namespace Never.IoC
{
    /// <summary>
    /// IoC容器接口
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// 服务注册器
        /// </summary>
        IServiceRegister ServiceRegister { get; }

        /// <summary>
        /// 服务定位器
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// 服务创建器
        /// </summary>
        IServiceActivator ServiceActivator { get; }

        /// <summary>
        /// 类型发现者
        /// </summary>
        ITypeFinder TypeFinder { get; }
    }
}