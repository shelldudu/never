using Never.Serialization;
using Never.WorkFlow.Attributes;
using System;
using System.Collections.Generic;

namespace Never.WorkFlow
{
    /// <summary>
    /// 工作流引擎
    /// </summary>
    public interface IWorkFlowEngine
    {
        /// <summary>
        /// 创建任务队列
        /// </summary>
        /// <returns></returns>
        ITaskschedQueue CreateQueue();

        /// <summary>
        /// 创建任务执行引擎
        /// </summary>
        /// <returns></returns>
        ITaskschedEngine CreateEngine();

        /// <summary>
        /// 模板引擎
        /// </summary>
        /// <returns></returns>
        ITemplateEngine TemplateEngine { get; }

        /// <summary>
        /// 任务策略
        /// </summary>
        ITaskschedStrategy Strategy { get; }

        /// <summary>
        /// json序列化内容
        /// </summary>
        IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// 工作执行情况
        /// </summary>
        WorkFlowStatus Status { get; }

        /// <summary>
        /// 工作步骤
        /// </summary>
        IEnumerable<KeyValuePair<WorkStepAttribute,Type>> WorkSteps { get; }
    }
}