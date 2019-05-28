using Never.Aop.DomainFilters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Commands
{
    /// <summary>
    /// CommandHandler执行的行为分析
    /// </summary>
    internal class CommandHandlerMethodProcessor : Never.Startups.ITypeProcessor
    {
        #region field

        /// <summary>
        /// 绑定方法
        /// </summary>
        private readonly static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// 命令上下文类型
        /// </summary>
        private readonly static Type cmdHandlerType = typeof(ICommandHandler<>);

        /// <summary>
        /// 定义事件
        /// </summary>
        private readonly static string defineEvent = typeof(ICommandHandler<>).GetMethods(bindingFlags)[0].Name;

        /// <summary>
        /// 命令上下文
        /// </summary>
        private readonly static Type commandContextTye = typeof(ICommandContext);

        #endregion field

        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!ObjectExtension.IsAssignableToType(cmdHandlerType, type))
                return;

            /*分析当前handlerType*/
            var handlerAttributes = type.GetCustomAttributes(true);
            var list = new List<Attribute>();
            foreach (var attr in handlerAttributes)
            {
                list.Add((Attribute)attr);
            }

            HandlerBehaviorStorager.Default.Add(type, list);

            /*分析excute方法块*/
            var methods = type.GetMethods(bindingFlags);
            foreach (var method in methods)
            {
                if (method.Name != defineEvent)
                    continue;

                var paras = method.GetParameters();
                if (paras.Length != 2)
                    continue;

                if (commandContextTye.IsAssignableFrom(paras[0].ParameterType))
                {
                    var objects = method.GetCustomAttributes(true);
                    if (objects == null)
                        return;

                    var attributes = new List<Attribute>();
                    foreach (var attr in objects)
                    {
                        attributes.Add((Attribute)attr);
                    }

                    HandlerBehaviorStorager.Default.Add(type, paras[1].ParameterType, attributes);
                }
            }
        }
    }
}