using System;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 每执行一步的信息
    /// </summary>
    [Serializable]
    public class ExecutingNode : IExecutingNode
    {
        #region prop

        /// <summary>
        /// 执行的主键
        /// </summary>
        public Guid RowId { get; set; }

        /// <summary>
        /// 任务的主键
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 节点排序
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 工作开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 工作完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 当前步骤类型，如果多个任务协同的，请以｜分割
        /// </summary>
        public string StepType { get; set; }

        /// <summary>
        /// 协同模式
        /// </summary>
        public CoordinationMode StepCoordinationMode { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailTimes { get; set; }

        /// <summary>
        /// 消息为空次数
        /// </summary>
        public int WaitTimes { get; set; }

        /// <summary>
        /// 步骤总条数
        /// </summary>
        public int StepCount { get; set; }

        /// <summary>
        /// 执行消息
        /// </summary>
        public string ExecutingMessage { get; set; }

        /// <summary>
        /// 执行的消息
        /// </summary>
        public string ResultMessage { get; set; }

        #endregion prop
    }
}