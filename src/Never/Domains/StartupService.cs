using Never.Commands;
using Never.Events;
using Never.Exceptions;
using Never.IoC;
using Never.Startups;
using System;
using System.Collections.Generic;

namespace Never.Domains
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    internal sealed class StartupService : Never.Startups.IStartupService
    {
        #region field

        /// <summary>
        /// 是否已经初始化了
        /// </summary>
        private static bool isInit = false;

        /// <summary>
        /// 生命周期
        /// </summary>
        private readonly ComponentLifeStyle lifeStyle = ComponentLifeStyle.Scoped;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupService"/> class.
        /// </summary>
        /// <param name="lifeStyle">生命周期</param>
        public StartupService(ComponentLifeStyle lifeStyle)
        {
            this.lifeStyle = lifeStyle;
        }

        #endregion ctor

        #region IStartupService

        /// <summary>
        /// 在行下问开始启动时刻，要处理的逻辑
        /// </summary>
        /// <param name="context">上下文</param>
        void Startups.IStartupService.OnStarting(Startups.StartupContext context)
        {
            if (context == null)
                throw new SystemBusyException("启动上下文为空，请确保其初始化");

            if (isInit)
                return;

            this.Register(context);

            /*分析聚合根事件*/
            context.ProcessType(new ITypeProcessor[4] { new EventHandlerMethodProcessor(), new CommandHandlerMethodProcessor(), new CommandTypeProcessor(), new EventTypeProcessor() });
            isInit = true;
        }

        /// <summary>
        /// 排序，该规则在第31位
        /// </summary>
        int Startups.IStartupService.Order
        {
            get { return 31; }
        }

        #endregion IStartupService

        #region utils

        /// <summary>
        /// 注册IoC对象
        /// </summary>
        /// <param name="context">上下文</param>
        private void Register(Startups.StartupContext context)
        {
            /*所有监听命令*/
            var commands = context.TypeFinder.FindClassesOfType(context.FilteringAssemblyProvider.GetAssemblies(), new[] { typeof(ICommandHandler<>) }) ?? new Type[] { };
            var commandType = typeof(ICommandHandler);
            var eventType = typeof(IEventHandler);
            var commandOnlyHandlers = new Dictionary<Type, Type>(commands.Length);
            foreach (var lister in commands)
            {
                foreach (var inter in lister.GetInterfaces())
                {
                    if (!inter.IsGenericType)
                        continue;

                    if (inter.GetInterface(commandType.Name) == null)
                        continue;

                    if (commandOnlyHandlers.ContainsKey(inter))
                        throw new ParameterException(inter.GetGenericArguments()[0].Name, "duplicate command handlers in {0}", inter.FullName);

                    context.ServiceRegister.RegisterType(lister, inter, string.Empty, this.lifeStyle);
                    commandOnlyHandlers[inter] = lister;
                }
            }

            /*所有监听事件*/
            var events = context.TypeFinder.FindClassesOfType(context.FilteringAssemblyProvider.GetAssemblies(),
                new[] { typeof(IEventHandler<>) } ?? new Type[] { });

            foreach (var lister in events)
            {
                foreach (var inter in lister.GetInterfaces())
                {
                    if (!inter.IsGenericType)
                        continue;

                    if (inter.GetInterface(eventType.Name) == null)
                        continue;

                    context.ServiceRegister.RegisterType(lister, inter, string.Empty, this.lifeStyle);
                }
            }
        }

        #endregion utils
    }
}