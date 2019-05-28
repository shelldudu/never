using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境提供者
    /// </summary>
    public class TransientAutoInjectingEnvironmentProvider : BaseAutoInjectingEnvironmentProvider
    {
        private readonly BaseAutoInjectingEnvironmentProvider provider = null;
        private readonly Func<String, AutoInjectingTurpeGroup, Object, ITypeFinder, IEnumerable<Assembly>, bool> match = null;
        private readonly string env = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public TransientAutoInjectingEnvironmentProvider(string env, BaseAutoInjectingEnvironmentProvider provider)
            : this(env, provider, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <param name="match"></param>
        public TransientAutoInjectingEnvironmentProvider(string env, BaseAutoInjectingEnvironmentProvider provider, Func<String, AutoInjectingTurpeGroup, Object, ITypeFinder, IEnumerable<Assembly>, bool> match)
        {
            this.env = env;
            this.provider = provider;
            this.match = match;
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="group"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        protected override bool Match(AutoInjectingTurpeGroup group, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            if (this.match != null)
                return this.match(this.env, group, @object, typeFinder, assemblies);

            var att = group.Attribute as TransientAutoInjectingAttribute;
            if (att == null)
                return false;

            return this.env.IsEquals(att.Env);
        }

        /// <summary>
        /// 要验证的自动注入属性
        /// </summary>
        /// <returns></returns>
        public override Type GetAutoInjectingAttributeType()
        {
            return typeof(TransientAutoInjectingAttribute);
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
            this.provider.Register(groups, @object, typeFinder, assemblies);
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
            this.provider.Register(groups, @object, typeFinder, assemblies);
        }

        /// <summary>
        /// 使用注册规则是<seealso cref="Never.IoC.Injections.RegisterRuleCollector"/>类型的方法信息
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static TransientAutoInjectingEnvironmentProvider UsingRuleContainerAutoInjectingEnvironmentProvider(string env)
        {
            return new TransientAutoInjectingEnvironmentProvider(env, new RuleCollectorAutoInjectingEnvironmentProvider());
        }
    }
}