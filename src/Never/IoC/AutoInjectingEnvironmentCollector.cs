using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境规则收集者
    /// </summary>
    internal struct AutoInjectingEnvironmentCollector : IAutoInjectingEnvironmentCollector
    {
        /// <summary>
        /// 收集类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="collector"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public void Register(AutoInjectingGroupInfo[] groups, object collector, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            var recollector = collector as IoC.Injections.RegisterRuleCollector;
            if (recollector != null)
            {
                foreach (var g in groups)
                    recollector.RegisterType(g.ImplementationType, g.Attribute.ServiceType, g.Key, g.LifeStyle);

                return;
            }

            var register = collector as IoC.IServiceRegister;
            if (register != null)
            {
                foreach (var g in groups)
                    register.RegisterType(g.ImplementationType, g.Attribute.ServiceType, g.Key, g.LifeStyle);

                return;
            }
        }
    }
}