using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 任务调整策略
    /// </summary>
    public interface ITaskschedStrategy
    {
        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        TimeSpan NextTime(ITaskschedNode taskNode, IExecutingNode excutingNode, Exception ex);

        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        /// <param name="messageNullTime">消息为空次数</param>
        TimeSpan NextWorkTimeOnMessageIsNull(ITaskschedNode taskNode, IExecutingNode excutingNode, int messageNullTime);

        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        TimeSpan NextWorkTimeOnMessageIsWaiting(ITaskschedNode taskNode, IExecutingNode excutingNode);
    }
}