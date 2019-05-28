using Never.WorkFlow.Coordinations;
using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 任务引擎
    /// </summary>
    public interface ITaskschedEngine
    {
        /// <summary>
        /// 执行该队列下所有任务
        /// </summary>
        /// <param name="taskId"></param>
        void Execute(Guid taskId);

        /// <summary>
        /// 执行该队列下所有任务
        /// </summary>
        /// <param name="task"></param>
        void Execute(TaskschedNode task);
    }
}