using Never.Aop.DomainFilters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Events
{
    /// <summary>
    /// EventHandler执行的行为分析
    /// </summary>
    internal class EventHandlerMethodProcessor : Never.Startups.ITypeProcessor
    {
        #region field

        /// <summary>
        /// 绑定方法
        /// </summary>
        private readonly static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// 事件类型
        /// </summary>
        private readonly static Type eventHandlerType = typeof(IEventHandler);

        /// <summary>
        /// 定义事件
        /// </summary>
        private readonly static string defineEvent = typeof(IEventHandler<>).GetMethods(bindingFlags)[0].Name;

        /// <summary>
        /// 命令上下文
        /// </summary>
        private readonly static Type eventContextTye = typeof(IEventContext);

        #endregion field

        #region ITypeProcessor

        /// <summary>
        ///
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!ObjectExtension.IsAssignableToType(eventHandlerType, type))
                return;

            /*分析当前handlerType*/
            var handlerAttributes = type.GetCustomAttributes(true);
            if (handlerAttributes != null)
            {
                var attrs = new List<Attribute>();
                foreach (var n in handlerAttributes)
                {
                    attrs.Add((Attribute)n);
                }

                HandlerBehaviorStorager.Default.Add(type, attrs);
            }

            /*分析excute方法块*/
            var methods = type.GetMethods(bindingFlags);
            foreach (var method in methods)
            {
                if (method.Name != defineEvent)
                    continue;

                var paras = method.GetParameters();
                if (paras.Length < 2 || paras.Length > 5)
                    continue;

                if (!eventContextTye.IsAssignableFrom(paras[0].ParameterType))
                    continue;

                var objects = method.GetCustomAttributes(true);
                if (objects == null)
                    return;

                var attributes = new List<Attribute>();
                foreach (var attr in objects)
                    attributes.Add((Attribute)attr);

                HandlerBehaviorStorager.Default.Add(type, paras[1].ParameterType, attributes);
            }
        }

        #endregion ITypeProcessor
    }
}