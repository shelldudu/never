using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境提供者
    /// </summary>
    public class ScopedAutoInjectingEnvironmentProvider : BaseAutoInjectingEnvironmentProvider
    {
        private readonly BaseAutoInjectingEnvironmentProvider provider = null;
        private readonly Func<String, AutoInjectingTurpeGroup, Object, ITypeFinder, IEnumerable<Assembly>, bool> match = null;
        private readonly string env = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public ScopedAutoInjectingEnvironmentProvider(string env, BaseAutoInjectingEnvironmentProvider provider)
            : this(env, provider, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <param name="match"></param>
        public ScopedAutoInjectingEnvironmentProvider(string env, BaseAutoInjectingEnvironmentProvider provider, Func<String, AutoInjectingTurpeGroup, Object, ITypeFinder, IEnumerable<Assembly>, bool> match)
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

            var att = group.Attribute as ScopedAutoInjectingAttribute;
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
            return typeof(ScopedAutoInjectingAttribute);
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
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static ScopedAutoInjectingEnvironmentProvider UsingRuleContainerAutoInjectingEnvironmentProvider(string env)
        {
            return new ScopedAutoInjectingEnvironmentProvider(env, new RuleCollectorAutoInjectingEnvironmentProvider());
        }
    }
}