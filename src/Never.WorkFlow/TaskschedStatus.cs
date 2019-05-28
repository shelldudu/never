namespace Never.WorkFlow
{
    /// <summary>
    /// 节点状态，注意在仓库中会固定某些值，所在修改的时候要注意点
    /// </summary>
    public enum TaskschedStatus : byte
    {
        /// <summary>
        /// 正在工作
        /// </summary>
        Working = 0,

        /// <summary>
        /// 完成
        /// </summary>
        Finish = 2,

        /// <summary>
        /// 中止计划
        /// </summary>
        Abort = 3,
    }
}