using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境规则收集者
    /// </summary>
    public interface IAutoInjectingEnvironmentCollector
    {
        /// <summary>
        /// 收集类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="collector"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        void Register(AutoInjectingGroupInfo[] groups, object collector, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies);
    }
}