using Never.Events;
using Never.Exceptions;
using Never.Startups;
using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 分析每个事件是否带上了<see cref="EventDomainAttribute"/>特性
    /// </summary>
    internal class EventAppDomainStartupService : Never.Startups.ITypeProcessor, Never.Startups.IStartupService
    {
        #region field

        /// <summary>
        /// 特性类型
        /// </summary>
        private readonly static Type attributeType = typeof(EventDomainAttribute);

        /// <summary>
        /// ddd事件
        /// </summary>
        private readonly static Type eventType = typeof(IEvent);

        #endregion field

        int IStartupService.Order
        {
            get
            {
                return 100056;
            }
        }

        void IStartupService.OnStarting(StartupContext context)
        {
            /*分析聚合根事件*/
            context.ProcessType(this);
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="application">启动宿主对象</param>
        /// <param name="type">对象类型</param>
        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!eventType.IsAssignableFrom(type))
                return;

            if (type.GetCustomAttributes(attributeType, false).Length > 0)
                return;

            throw new InvalidException("事件{0}没有配置上EventDomainAttribute特性", type.FullName);
        }
    }
}