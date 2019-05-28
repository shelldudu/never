using Never.Events;
using Never.Startups;
using System;
using System.Reflection;

namespace Never.Domains
{
    /// <summary>
    /// 分析每个聚合根中指定的聚合根事件,该事件的定义是 IHandle &lt; T &gt; 中定义的
    /// </summary>
    internal class AggregateRootIHandleStartService : Never.Startups.ITypeProcessor, Never.Startups.IStartupService
    {
        #region field

        /// <summary>
        /// 绑定方法
        /// </summary>
        private readonly static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// 聚合根类型
        /// </summary>
        private readonly static Type rootType = typeof(IAggregateRoot);

        /// <summary>
        /// ddd事件
        /// </summary>
        private readonly static Type eventType = typeof(IEvent);

        /// <summary>
        /// 定义事件
        /// </summary>
        private readonly static string defineEvent = typeof(IHandle<>).GetMethods(bindingFlags)[0].Name;

        #endregion field

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="application">启动宿主对象</param>
        /// <param name="type">对象类型</param>
        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!rootType.IsAssignableFrom(type))
                return;

            var events = type.GetMethods(bindingFlags);
            foreach (var e in events)
            {
                if (e.Name != defineEvent)
                    continue;

                var paras = e.GetParameters();
                if (paras.Length != 1)
                    continue;

                if (!eventType.IsAssignableFrom(paras[0].ParameterType))
                    continue;

                if (ObjectExtension.IsAssignableToType(typeof(IHandle<>).MakeGenericType(paras[0].ParameterType), type))
                    continue;

                throw new Exception(string.Format("聚合根{0}找到Handle<{1}>方法，但它没有实现IHandle<{1}>接口", type.FullName, paras[0].ParameterType.Name));
            }
        }

        int IStartupService.Order
        {
            get
            {
                return 11220;
            }
        }

        void IStartupService.OnStarting(StartupContext context)
        {
            context.ProcessType(this);
        }
    }
}