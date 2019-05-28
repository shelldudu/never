namespace Never.Domains
{
    /// <summary>
    /// 聚合根发生变更时定义的事件约定接口
    /// </summary>
    /// <typeparam name="TEvent">领域事件类型</typeparam>
    public interface IHandle<in TEvent> where TEvent : Never.Events.IEvent
    {
        /// <summary>
        /// 处理领域事件
        /// </summary>
        /// <param name="e">领域事件类型</param>
        void Handle(TEvent e);
    }
}