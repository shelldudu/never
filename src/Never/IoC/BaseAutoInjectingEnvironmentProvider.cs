using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 基本主动注入环境提供者
    /// </summary>
    public abstract class BaseAutoInjectingEnvironmentProvider : IAutoInjectingEnvironmentProvider
    {
        /// <summary>
        /// 要验证的自动注入属性
        /// </summary>
        /// <returns></returns>
        public abstract Type GetAutoInjectingAttributeType();

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="group"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        protected virtual bool Match(AutoInjectingTurpeGroup group, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies)
        {
            return true;
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="eventArgs"></param>
        public void Call(AutoInjectingTurpeGroup[] groups, IContainerStartupEventArgs eventArgs)
        {
            var list = new List<AutoInjectingTurpeGroup>(groups.Where(o => this.Match(o, eventArgs.Collector, eventArgs.TypeFinder, eventArgs.Assemblies)));
            var lifeStyle = list.Where(o => o.Attribute is LifeStyleAutoInjectingAttribute).ToArray();
            if (lifeStyle.Length > 0)
            {
                this.Register(lifeStyle.Select(n => new AutoInjectingQuaternaryGroup() { Attribute = n.Attribute, ImplementationType = n.ImplementationType, Key = ((LifeStyleAutoInjectingAttribute)n.Attribute).Key, LifeStyle = ((LifeStyleAutoInjectingAttribute)n.Attribute).Declare }).ToArray(), eventArgs.Collector, eventArgs.TypeFinder, eventArgs.Assemblies);
                foreach (var s in lifeStyle)
                    list.Remove(s);
            }

            this.Register(list.ToArray(), eventArgs.Collector, eventArgs.TypeFinder, eventArgs.Assemblies);
        }

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public abstract void Register(AutoInjectingQuaternaryGroup[] groups, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies);

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="object"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblies"></param>
        public abstract void Register(AutoInjectingTurpeGroup[] groups, object @object, ITypeFinder typeFinder, IEnumerable<Assembly> assemblies);
    }
}