namespace Never.WorkFlow
{
    /// <summary>
    /// 工作状态
    /// </summary>
    public enum WorkFlowStatus : byte
    {
        /// <summary>
        /// 正在初始化
        /// </summary>
        Initing = 0,

        /// <summary>
        /// 已启动
        /// </summary>
        Started = 1,

        /// <summary>
        /// 忆关闭
        /// </summary>
        Stoped = 2,
    }
}