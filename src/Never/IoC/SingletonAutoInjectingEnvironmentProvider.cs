using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境提供者
    /// </summary>
    public class SingletonAutoInjectingEnvironmentProvider : IAutoInjectingEnvironmentProvider
    {
        private readonly IAutoInjectingEnvironmentCollector collector = null;
        private readonly Func<String, AutoInjectingGroupInfo, Object, ITypeFinder, IEnumerable<Assembly>, bool> match = null;
        private readonly string env = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="collector"></param>
        public SingletonAutoInjectingEnvironmentProvider(string env, IAutoInjectingEnvironmentCollector collector)
            : this(env, collector, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="collector"></param>
        /// <param name="match"></param>
        public SingletonAutoInjectingEnvironmentProvider(string env, IAutoInjectingEnvironmentCollector collector, Func<String, AutoInjectingGroupInfo, Object, ITypeFinder, IEnumerable<Assembly>, bool> match)
        {
            this.env = env;
            this.collector = collector;
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
        protected bool Match(AutoInjectingGroupInfo group, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            if (this.match != null)
                return this.match(this.env, group, @object, typeFinder, assemblies);

            var att = group.Attribute as SingletonAutoInjectingAttribute;
            if (att == null)
                return false;

            return this.env.IsEquals(att.Env);
        }

        /// <summary>
        /// 要验证的自动注入属性
        /// </summary>
        /// <returns></returns>
        public Type GetAutoInjectingAttributeType()
        {
            return typeof(SingletonAutoInjectingAttribute);
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="eventArgs"></param>
        public void Call(AutoInjectingGroupInfo[] groups, IContainerStartupEventArgs eventArgs)
        {
            var lifeStyle = new List<AutoInjectingGroupInfo>(groups.Where(o => this.Match(o, eventArgs.Collector, eventArgs.TypeFinder, eventArgs.Assemblies)));
            if (lifeStyle.IsNotNullOrEmpty())
            {
                this.collector.Register(lifeStyle.ToArray(), eventArgs.Collector, eventArgs.TypeFinder, eventArgs.Assemblies);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static SingletonAutoInjectingEnvironmentProvider UsingRuleContainerAutoInjectingEnvironmentProvider(string env)
        {
            return new SingletonAutoInjectingEnvironmentProvider(env, new AutoInjectingEnvironmentCollector());
        }
    }
}