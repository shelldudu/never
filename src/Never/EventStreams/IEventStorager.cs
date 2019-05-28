using Never.Commands;
using Never.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EventStreams
{
    /// <summary>
    /// 领域事件存储接口
    /// </summary>
    public interface IEventStorager
    {
        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        void Save(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> events);
    }
}
