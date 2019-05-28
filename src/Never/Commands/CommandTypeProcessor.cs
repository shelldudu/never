using Never.Aop.DomainFilters;
using Never.Commands.Concurrent;
using Never.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// Command特性分析
    /// </summary>
    internal class CommandTypeProcessor : Never.Startups.ITypeProcessor
    {
        #region field

        /// <summary>
        /// 命令类型
        /// </summary>
        private readonly static Type commandType = typeof(ICommand);

        /// <summary>
        /// 并行处理命令类型
        /// </summary>
        private readonly static Type serialeCommandType = typeof(ISerialCommand);

        #endregion field

        void Startups.ITypeProcessor.Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!ObjectExtension.IsAssignableToType(commandType, type))
                return;

            /*并行命令处理*/
            if (ObjectExtension.IsAssignableToType(serialeCommandType, type))
            {
                Database.Create(type);
            }

            /*分析当前handlerType*/
            var handlerAttributes = type.GetCustomAttributes(true);
            if (handlerAttributes == null)
                return;

            var list = new List<Attribute>();
            foreach (var attr in handlerAttributes)
            {
                list.Add((Attribute)attr);
            }

            CommandBehaviorStorager.Default.Add(type, list);

            foreach (var attr in handlerAttributes)
            {
                var v = attr as ValidatorAttribute;
                if (v == null)
                    continue;

                application.ServiceRegister.RegisterType(v.ValidatorType, v.ValidatorType, string.Empty, IoC.ComponentLifeStyle.Transient);
            }
        }
    }
}