using Never.Startups;
using System.Collections.Generic;
using System.Linq;

namespace Never.IoC
{
    /// <summary>
    ///
    /// </summary>
    internal class AutoInjectingStartupService : Never.Startups.IStartupService
    {
        #region field

        private readonly IAutoInjectingEnvironmentProvider[] providers = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="providers"></param>
        public AutoInjectingStartupService(IAutoInjectingEnvironmentProvider[] providers)
        {
            this.providers = providers;
        }

        #endregion ctor

        /// <summary>
        /// 排序
        /// </summary>
        public int Order
        {
            get
            {
                return 2;
            }
        }

        public void OnStarting(StartupContext context)
        {
            if (this.providers == null || this.providers.Length <= 0)
                return;

            var repeat = context.Container as IContainerStartup;
            if (repeat == null)
                return;

            var dict = new List<AutoInjectingGroupInfo>(50);
            foreach (var assembly in context.FilteringAssemblyProvider.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(AutoInjectingAttribute), true);
                    if (attributes == null || attributes.Length <= 0)
                        continue;

                    foreach (AutoInjectingAttribute attribute in attributes)
                    {
                        dict.Add(new AutoInjectingGroupInfo() { Attribute = attribute, ImplementationType = type, Key = attribute.Key, LifeStyle = attribute.Declare });
                    }
                }
            }

            foreach (var p in this.providers)
            {
                var attributeType = p.GetAutoInjectingAttributeType();
                if (attributeType == null)
                    continue;

                var groups = dict.Where(o => o.Attribute.GetType() == attributeType).ToArray();
                repeat.OnStarting += (s, e) => p.Call(groups, e);
            }
        }
    }
}