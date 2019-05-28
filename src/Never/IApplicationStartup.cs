using Never.IoC;
using Never.Startups;
using System;
using System.Collections.Generic;

namespace Never
{
    /// <summary>
    /// 程序宿主环境配置服务
    /// </summary>
    public interface IApplicationStartup : IDisposable
    {
        /// <summary>
        /// 提供已过滤程序集提供者
        /// </summary>
        IFilteringAssemblyProvider FilteringAssemblyProvider { get; }

        /// <summary>
        /// 类型查找
        /// </summary>
        ITypeFinder TypeFinder { get; }

        /// <summary>
        /// 服务定位器
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// 服务注册器
        /// </summary>
        IServiceRegister ServiceRegister { get; }

        /// <summary>
        /// 上下文
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// 当前是否为web host环境
        /// </summary>
        bool IsWebHosted { get; }
    }
}