using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 容器初始化过程事件
    /// </summary>
    public class IContainerStartupEventArgs : EventArgs
    {
        /// <summary>
        /// 类型发现者
        /// </summary>
        public ITypeFinder TypeFinder { get; }

        /// <summary>
        /// 程序集
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; }

        /// <summary>
        /// app
        /// </summary>
        public object Collector { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        /// <param name="collector"></param>
        public IContainerStartupEventArgs(ITypeFinder typeFinder, IEnumerable<Assembly> assemblies, object collector)
        {
            this.TypeFinder = typeFinder;
            this.Assemblies = assemblies;
            this.Collector = collector;
        }
    }
}