using Never.Aop.DomainFilters;
using System;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// Events特性分析
    /// </summary>
    internal class EventTypeProcessor : Never.Startups.ITypeProcessor
    {
        #region field

        /// <summary>
        /// 事件类型
        /// </summary>
        private readonly static Type eventType = typeof(IEvent);

        #endregion field

        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!ObjectExtension.IsAssignableToType(eventType, type))
                return;

            /*分析当前handlerType*/
            var handlerAttributes = type.GetCustomAttributes(true);
            if (handlerAttributes == null && handlerAttributes.Length == 0)
                return;

            var list = new List<Attribute>(handlerAttributes.Length);
            foreach (var attr in handlerAttributes)
            {
                list.Add((Attribute)attr);
            }

            EventBehaviorStorager.Default.Add(type, list);
        }
    }
}