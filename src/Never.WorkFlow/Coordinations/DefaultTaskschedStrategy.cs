using System;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 任务调整策略,所有任务默认返回5分钟
    /// </summary>
    public class DefaultTaskschedStrategy : ITaskschedStrategy
    {
        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        public virtual TimeSpan NextTime(ITaskschedNode taskNode, IExecutingNode excutingNode, Exception ex)
        {
            return TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        /// <param name="messageNullTime">消息为空次数</param>
        public virtual TimeSpan NextWorkTimeOnMessageIsNull(ITaskschedNode taskNode, IExecutingNode excutingNode, int messageNullTime)
        {
            return TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// 下一次作业时间
        /// </summary>
        /// <param name="taskNode">任务节点</param>
        /// <param name="excutingNode">执行详情节点</param>
        public virtual TimeSpan NextWorkTimeOnMessageIsWaiting(ITaskschedNode taskNode, IExecutingNode excutingNode)
        {
            return TimeSpan.FromMinutes(5);
        }
    }
}