using Never.IoC;
using Never.Logging;
using Never.Messages;
using Never.Serialization;
using Never.WorkFlow.Attributes;
using Never.WorkFlow.Coordinations;
using Never.WorkFlow.Exceptions;
using Never.WorkFlow.Repository;
using System;
using System.Collections.Generic;

namespace Never.WorkFlow
{
    /// <summary>
    /// 工作流引擎
    /// </summary>
    public class WorkFlowEngine : IWorkFlowEngine, IWorkService
    {
        #region field

        /// <summary>
        /// 模板仓库
        /// </summary>
        private readonly ITemplateRepository templateRepository = null;

        /// <summary>
        /// 执行仓库
        /// </summary>
        private readonly IExecutingRepository executingRepository = null;

        /// <summary>
        /// 并发策略
        /// </summary>
        private readonly ISerialTaskStrategy serialTaskStrategy = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="executingRepository"></param>
        /// <param name="templateRepository"></param>
        /// <param name="jsonSerializer"></param>
        public WorkFlowEngine(ITemplateRepository templateRepository, IExecutingRepository executingRepository, IJsonSerializer jsonSerializer)
            : this(templateRepository, executingRepository, jsonSerializer, new DefaultTaskschedStrategy(), new DefaultSerialTaskStrategy())
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="templateRepository"></param>
        /// <param name="executingRepository"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="taskschedStrategy"></param>
        /// <param name="serialTaskStrategy"></param>
        public WorkFlowEngine(ITemplateRepository templateRepository, IExecutingRepository executingRepository, IJsonSerializer jsonSerializer, ITaskschedStrategy taskschedStrategy, ISerialTaskStrategy serialTaskStrategy)
        {
            this.templateRepository = templateRepository;
            this.executingRepository = executingRepository;
            this.serialTaskStrategy = serialTaskStrategy;

            this.JsonSerializer = jsonSerializer;
            this.Strategy = taskschedStrategy;
            this.Status = WorkFlowStatus.Initing;
            this.TemplateEngine = new TemplateEngine(this, templateRepository);
        }


        #endregion ctor

        #region start

        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Startup()
        {
            if (this.Status == WorkFlowStatus.Started)
            {
                return;
            }

            if (this.Status == WorkFlowStatus.Stoped)
            {
                return;
            }

            if (this.templateRepository == null)
            {
                throw new WorkFlowEngineInitException("the templateRepository is null,workflow can not init");
            }

            if (this.executingRepository == null)
            {
                throw new WorkFlowEngineInitException("the executingRepository is null,workflow can not init");
            }

            if (this.JsonSerializer == null)
            {
                throw new WorkFlowEngineInitException("the jsonSerializer is null,workflow can not init");
            }

            if (this.TemplateEngine == null)
            {
                throw new WorkFlowEngineInitException("the template is null,workflow can not init");
            }

            this.TemplateEngine.Startup();
            this.Status = WorkFlowStatus.Started;
        }

        /// <summary>
        /// 工作停止了
        /// </summary>
        public void Shutdown()
        {
            if (this.Status == WorkFlowStatus.Stoped)
            {
                return;
            }

            this.Status = WorkFlowStatus.Stoped;
        }

        #endregion start

        #region engine

        /// <summary>
        /// 模板引擎
        /// </summary>
        public ITemplateEngine TemplateEngine { get; }

        /// <summary>
        /// 创建任务队列
        /// </summary>
        /// <returns></returns>
        public virtual ITaskschedQueue CreateQueue()
        {
            return new TaskschedQueue(this, this.executingRepository, this.templateRepository);
        }

        /// <summary>
        /// 创建任务执行引擎
        /// </summary>
        public virtual ITaskschedEngine CreateEngine()
        {
            return new TaskschedEngine(this, this.executingRepository)
            {
                MessagePublisher = ContainerContext.Current?.ServiceLocator?.ResolveOptional<IMessagePublisher>(),
                SerialTaskStrategy = this.serialTaskStrategy
            };
        }

        /// <summary>
        /// 任务策略
        /// </summary>
        public ITaskschedStrategy Strategy { get; }

        /// <summary>
        /// 启动程序
        /// </summary>
        public ApplicationStartup ApplicationStartup { get; set; }

        /// <summary>
        /// 工作执行情况
        /// </summary>
        public WorkFlowStatus Status { get; private set; }

        /// <summary>
        /// json序列化内容
        /// </summary>
        public IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// 工作步骤
        /// </summary>
        public IEnumerable<KeyValuePair<WorkStepAttribute, Type>> WorkSteps => TemplateWorkStep.Steps;

        #endregion engine
    }
}