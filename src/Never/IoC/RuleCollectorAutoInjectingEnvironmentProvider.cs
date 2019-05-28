using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境提供者，在验证过程中，如果@object对象是<seealso cref="Never.IoC.Injections.RegisterRuleCollector"/>则生效
    /// </summary>
    internal class RuleCollectorAutoInjectingEnvironmentProvider : BaseAutoInjectingEnvironmentProvider
    {
        /// <summary>
        /// 不实现任何操作的信息
        /// </summary>
        /// <returns></returns>
        public override Type GetAutoInjectingAttributeType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public override void Register(AutoInjectingQuaternaryGroup[] groups, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            var rule = @object as IoC.Injections.RegisterRuleCollector;
            if (rule != null)
            {
                foreach (var g in groups)
                    rule.RegisterType(g.ImplementationType, g.Attribute.ServiceType, g.Key, g.LifeStyle);
            }
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public override void Register(AutoInjectingTurpeGroup[] groups, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            var rule = @object as IoC.Injections.RegisterRuleCollector;
            if (rule != null)
            {
                foreach (var g in groups)
                    rule.RegisterType(g.ImplementationType, g.Attribute.ServiceType);
            }
        }
    }
}