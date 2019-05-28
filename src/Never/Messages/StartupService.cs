using Never.Exceptions;
using Never.IoC;

namespace Never.Messages
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    internal class StartupService : Never.Startups.IStartupService
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
            /*所有监听消息*/
            var messages = context.TypeFinder.FindClassesOfType(context.FilteringAssemblyProvider.GetAssemblies(),
                new[] { typeof(IMessageSubscriber<>) });

            var eventType = typeof(IMessageSubscriber);
            foreach (var lister in messages)
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