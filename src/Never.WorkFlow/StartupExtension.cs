using Never.IoC;
using Never.Logging;
using Never.Serialization;
using Never.WorkFlow.Coordinations;
using Never.WorkFlow.Repository;
using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        #region workflow

        /// <summary>
        /// 启动工作流
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="executingRepository"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="templateRepository"></param>
        /// <param name="environmentInit"></param>
        /// <returns></returns>
        public static ApplicationStartup UseWorlFlow(this ApplicationStartup startup,
            ITemplateRepository templateRepository,
            IExecutingRepository executingRepository,
            IJsonSerializer jsonSerializer,
            Action<IWorkFlowEngine> environmentInit)
        {
            if (startup.Items.ContainsKey("UseWorlFlow"))
            {
                return startup;
            }

            startup.ServiceRegister.RegisterInstance<ITemplateRepository>(templateRepository);
            startup.ServiceRegister.RegisterInstance<IExecutingRepository>(executingRepository);

            var engine = new WorkFlowEngine(templateRepository, executingRepository, jsonSerializer)
            {
                ApplicationStartup = startup,
            };
            startup.ServiceRegister.RegisterInstance<IWorkFlowEngine>(engine);
            startup.RegisterStartService(new StartupService(environmentInit, engine));
            startup.Items["UseWorlFlow"] = "true";
            return startup;
        }

        /// <summary>
        /// 启动工作流
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="executingRepository"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="templateRepository"></param>
        /// <param name="serialTaskStrategy"></param>
        /// <param name="taskschedStrategy"></param>
        /// <param name="environmentInit"></param>
        /// <returns></returns>
        public static ApplicationStartup UseWorlFlow(this ApplicationStartup startup,
            ITemplateRepository templateRepository,
            IExecutingRepository executingRepository,
            IJsonSerializer jsonSerializer,
            ITaskschedStrategy taskschedStrategy,
            ISerialTaskStrategy serialTaskStrategy,
            Action<IWorkFlowEngine> environmentInit)
        {
            if (startup.Items.ContainsKey("UseWorlFlow"))
            {
                return startup;
            }

            startup.ServiceRegister.RegisterInstance<ITemplateRepository>(templateRepository);
            startup.ServiceRegister.RegisterInstance<IExecutingRepository>(executingRepository);

            var engine = new WorkFlowEngine(templateRepository, executingRepository, jsonSerializer, taskschedStrategy ?? new DefaultTaskschedStrategy(), serialTaskStrategy ?? new DefaultSerialTaskStrategy())
            {
                ApplicationStartup = startup,
            };

            startup.ServiceRegister.RegisterInstance<IWorkFlowEngine>(engine);
            startup.RegisterStartService(new StartupService(environmentInit, engine));
            startup.Items["UseWorlFlow"] = "true";
            return startup;
        }

        #endregion workflow
    }
}