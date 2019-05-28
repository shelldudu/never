namespace Never.WorkFlow
{
    /// <summary>
    /// 任务搜索节点
    /// </summary>
    public class TaskschedNodeSearch : PagedSearch
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public string UserSign { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 工作状态
        /// </summary>
        public TaskschedStatus[] StatusArray { get; set; }
    }
}