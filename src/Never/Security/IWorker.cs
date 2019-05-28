namespace Never.Security
{
    /// <summary>
    /// 作业操作者
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// 操作者名字
        /// </summary>
        string WorkerName { get; }

        /// <summary>
        /// 操作者Id
        /// </summary>
        long WorkerId { get; }
    }
}