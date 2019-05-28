using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 领域事件存储接口
    /// </summary>
    public interface IEventStreamStorager
    {
        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        void Save(IEnumerable<IOperateEvent> events);
    }
}