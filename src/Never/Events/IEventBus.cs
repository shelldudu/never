using Never.Commands;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// C端事件总线
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="events"></param>
        void Push(ICommandContext context, IEnumerable<IEvent[]> events);

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="commandContext"></param>
        /// <param name="e"></param>
        void Publish<TEvent>(ICommandContext commandContext, TEvent e) where TEvent : IEvent;
    }
}