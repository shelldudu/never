using System;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 任务节点信息，在重新开始任务的时候创建出来
    /// </summary>
    [Serializable]
    public class TaskschedNode : ITaskschedNode
    {
        /// <summary>
        /// 执行的主键
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 工作开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 下一步开始工作时间
        /// </summary>
        public DateTime? NextTime { get; set; }

        /// <summary>
        /// 工作完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TaskschedStatus Status { get; set; }

        /// <summary>
        /// 步骤总条数
        /// </summary>
        public int StepCount { get; set; }

        /// <summary>
        /// 下一步执行的步骤
        /// </summary>
        public int NextStep { get; set; }

        /// <summary>
        /// 身份标识
        /// </summary>
        public string UserSign { get; set; }

        /// <summary>
        /// 身份标识标识
        /// </summary>
        public int UserSignState { get; set; }

        /// <summary>
        /// 附加状态
        /// </summary>
        public int AttachState { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 当前完成百分比
        /// </summary>
        public decimal ProcessPercent { get; set; }

        /// <summary>
        /// 第一条消息
        /// </summary>
        public string PushMessage { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public TaskschedNode()
        {
            this.StartTime = DateTime.Now.AddSeconds(10);
        }
    }
}