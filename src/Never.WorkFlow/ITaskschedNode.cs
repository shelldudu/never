using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 任务元素
    /// </summary>
    public interface ITaskschedNode
    {
        /// <summary>
        /// 执行的主键
        /// </summary>
        Guid TaskId { get; }

        /// <summary>
        /// 工作开始时间
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 下一步开始工作时间
        /// </summary>
        DateTime? NextTime { get; }

        /// <summary>
        /// 状态
        /// </summary>
        TaskschedStatus Status { get; }

        /// <summary>
        /// 工作完成时间
        /// </summary>
        DateTime? FinishTime { get; }

        /// <summary>
        /// 步骤总条数
        /// </summary>
        int StepCount { get; }

        /// <summary>
        /// 下一步执行的步骤
        /// </summary>
        int NextStep { get; }

        /// <summary>
        /// 身份标识
        /// </summary>
        string UserSign { get; }

        /// <summary>
        /// 附加状态
        /// </summary>
        int AttachState { get; }

        /// <summary>
        /// 命令类型
        /// </summary>
        string CommandType { get; }

        /// <summary>
        /// 当前完成百分比
        /// </summary>
        decimal ProcessPercent { get; }

        /// <summary>
        /// 第一条消息
        /// </summary>
        string PushMessage { get; }
    }
}