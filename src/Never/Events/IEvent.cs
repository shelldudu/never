namespace Never.Events
{
    /// <summary>
    /// 事件
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }
    }
}