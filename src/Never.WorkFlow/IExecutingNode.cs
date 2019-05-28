using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 执行元素
    /// </summary>
    public interface IExecutingNode
    {
        #region prop

        /// <summary>
        /// 任务的主键
        /// </summary>
        Guid TaskId { get; }

        /// <summary>
        /// 执行的主键
        /// </summary>
        Guid RowId { get; }

        /// <summary>
        /// 节点排序
        /// </summary>
        int OrderNo { get; }

        /// <summary>
        /// 工作开始时间
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// 工作完成时间
        /// </summary>
        DateTime? FinishTime { get; }

        /// <summary>
        /// 当前步骤类型，如果多个任务协同的，请以｜分割
        /// </summary>
        string StepType { get; }

        /// <summary>
        /// 协同模式
        /// </summary>
        CoordinationMode StepCoordinationMode { get; }

        /// <summary>
        /// 失败次数
        /// </summary>
        int FailTimes { get; }

        /// <summary>
        /// 消息为空次数
        /// </summary>
        int WaitTimes { get; }

        /// <summary>
        /// 执行消息
        /// </summary>
        string ExecutingMessage { get; }

        /// <summary>
        /// 步骤总条数
        /// </summary>
        int StepCount { get; }

        /// <summary>
        /// 执行的消息
        /// </summary>
        string ResultMessage { get; }


        #endregion prop
    }
}