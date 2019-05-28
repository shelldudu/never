namespace Never.Events
{
    /// <summary>
    /// 事件处理接口
    /// </summary>
    public interface IEventHandler
    {
    }

    /// <summary>
    /// 事件处理接口
    /// </summary>
    /// <typeparam name="TEvent">领域类型</typeparam>
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : IEvent
    {
        /// <summary>
        /// 事件处理约定
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="e">领域事件</param>
        void Execute(IEventContext context, TEvent e);
    }
}