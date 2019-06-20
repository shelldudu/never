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
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public void Register(AutoInjectingGroupInfo[] groups, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            var collector = @object as IoC.Injections.RegisterRuleCollector;
            if (collector != null)
            {
                foreach (var g in groups)
                    collector.RegisterType(g.ImplementationType, g.Attribute.ServiceType, g.Key, g.LifeStyle);

                return;
            }

            var register = @object as IoC.IServiceRegister;
            if (register != null)
            {
                foreach (var g in groups)
                    register.RegisterType(g.ImplementationType, g.Attribute.ServiceType, g.Key, g.LifeStyle);

                return;
            }
        }
    }
}