using Never.Messages;
using Never.Startups;
using Never.WorkFlow.Attributes;
using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 启动服务
    /// </summary>
    internal class StartupService : IStartupService, ITypeProcessor
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly Action<IWorkFlowEngine> environmentInit = null;

        /// <summary>
        ///
        /// </summary>
        private readonly WorkFlowEngine workFlowEngine = null;

        #endregion field

        #region ctor

        public StartupService(Action<IWorkFlowEngine> environmentInit, WorkFlowEngine workFlowEngine)
        {
            this.workFlowEngine = workFlowEngine;
            this.environmentInit = environmentInit;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// 启动的时候
        /// </summary>
        /// <param name="context"></param>
        public void OnStarting(StartupContext context)
        {
            context.ProcessType(this);
            this.environmentInit.Invoke(this.workFlowEngine);
            this.workFlowEngine.Startup();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        public void Processing(IApplicationStartup application, Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition || type.IsValueType)
            {
                return;
            }

            if (!ObjectExtension.IsAssignableToType(typeof(IWorkStep), type))
            {
                return;
            }

            TemplateWorkStep.AddStep(type);

            application.ServiceRegister.RegisterType(type, type, string.Empty, IoC.ComponentLifeStyle.Transient);
        }

        #endregion ctor
    }
}